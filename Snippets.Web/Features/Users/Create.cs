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
                RuleFor(d => d.Email)
                    .NotEmpty().WithMessage("Email can not be empty")
                    .EmailAddress().WithMessage("Email has be a propper email address");
                RuleFor(d => d.Password)
                    .NotEmpty().WithMessage("Password can not be empty")
                    .MinimumLength(12).WithMessage("Password has to be at least 12 characters long")
                    .Matches("[A-Z]").WithMessage("Password has to have at least one uppercase letter")
                    .Matches("[a-z]").WithMessage("Password has to have at least one lowercase letter")
                    .Matches("[0-9]").WithMessage("Password has to have at least one number")
                    .Matches("[!\"#$%&'()*+´\\-./:;<=>?@[\\]^_`{|}~]").WithMessage("Password has to have at least one special character"); 
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
                RuleFor(c => c.User)
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

            public Handler(SnippetsContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _jwtTokenGenerator = jwtTokenGenerator;
                _mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                if (await  _context.Persons.Where(u => u.Email == message.User.Email).AnyAsync(cancellationToken))
                    throw new RestException(HttpStatusCode.BadRequest, "Email already in use");

                var salt = Guid.NewGuid().ToByteArray();
                var person = new Person
                {
                    Email = message.User.Email,
                    DisplayName = message.User.DisplayName,
                    PasswordHash = _passwordHasher.Hash(message.User.Password, salt),
                    PasswordSalt = salt
                };

                _context.Persons.Add(person);
                await _context.SaveChangesAsync(cancellationToken);
                var user = _mapper.Map<Person, User>(person);
                var jwtToken = await _jwtTokenGenerator.CreateToken(person.PersonId);
                user.Tokens = new UserTokens {
                    Token = jwtToken,
                    RefreshToken = await _jwtTokenGenerator.CreateRefreshToken(jwtToken)
                };
                return new UserEnvelope(user);
            }
        }
    }
}
