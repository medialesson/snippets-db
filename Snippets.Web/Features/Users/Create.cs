using AutoMapper;
using FluentValidation;
using MediatR;
using Snippets.Web.Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common.Security;
using Snippets.Web.Domains;
using Snippets.Web.Common.Exceptions;
using System.Net;
using Snippets.Web.Common.Services;
using System.IO;

namespace Snippets.Web.Features.Users
{
    public class Create
    {
        public class UserData
        {
            public string Email { get; set; }

            public string DisplayName { get; set; }

            public string Password { get; set; }
        }

        public class UserDataValidator : AbstractValidator<UserData>
        {
            public UserDataValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email has to have a value")
                    .EmailAddress().WithMessage("Email has be a propper email address");


                RuleFor(x => x.Password)
#if DEBUG
                    .NotEmpty().WithMessage("Password has to have a value");
#else
                    .NotEmpty().WithMessage("Password has to have a value")
                    .MinimumLength(12).WithMessage("Password has to be at least 12 characters long")
                    .Matches("[A-Z]").WithMessage("Password has to have at least one uppercase letter")
                    .Matches("[a-z]").WithMessage("Password has to have at least one lowercase letter")
                    .Matches("[0-9]").WithMessage("Password has to have at least one number")
                    .Matches(@"[^\w\d]").WithMessage("Password has to have at least one special character");
#endif
            }
        }

        public class Command : IRequest<UserEnvelope>
        {
            public UserData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.User)
                    .NotNull().WithMessage("Payload has to contain a user object")
                    .SetValidator(new UserDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, UserEnvelope>
        {
            readonly SnippetsContext _context;
            readonly IPasswordHasher _passwordHasher;
            readonly IJwtTokenGenerator _jwtTokenGenerator;
            readonly IMapper _mapper;
            readonly IMailService _mailService;

            public Handler(SnippetsContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper, IMailService mailService)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _jwtTokenGenerator = jwtTokenGenerator;
                _mapper = mapper;
                _mailService = mailService;
            }

            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                if (await  _context.Persons.Where(u => u.Email == message.User.Email).AnyAsync(cancellationToken))
                    throw RestException.CreateFromDictionary(HttpStatusCode.BadRequest, new Dictionary<string, string> 
                    {
                        {"user.email", $"Email '{ message.User.Email }' is already in use"}
                    });

                var salt = Guid.NewGuid().ToByteArray();
                var person = new Person
                {
                    Email = message.User.Email,
                    DisplayName = message.User.DisplayName,
                    PasswordHash = _passwordHasher.Hash(message.User.Password, salt),
                    PasswordSalt = salt
                };

                var jwtToken = await _jwtTokenGenerator.CreateToken(person.PersonId);
                var refreshToken =  await _jwtTokenGenerator.CreateRefreshToken(jwtToken);
                person.RefreshToken = refreshToken.Split('.')[2];

                _context.Persons.Add(person);
                await _context.SaveChangesAsync(cancellationToken);

                var user = _mapper.Map<Person, User>(person);
                user.Tokens = new UserTokens {
                    Token = jwtToken,
                    Refresh = refreshToken 
                };

                await _mailService.SendEmailFromEmbeddedAsync(user.Email, "Welcome to Snippets DB", 
                    $"Snippets.Web.Views.Emails.Registration.cshtml", new Views.Emails.RegistrationModel
                    {
                        DisplayName = user.DisplayName,
                        VerificationUrl = "https://www.youtube.com/watch?v=DLzxrzFCyOs"
                    });

                return new UserEnvelope(user);
            }
        }
    }
}
