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
using Hangfire;

namespace Snippets.Web.Features.Users
{
    public class Create
    {
        public class UserData
        {
            /// <summary>
            /// New email to be associated with the Person
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// New name that gets displayed to other users associated with the Person 
            /// </summary>
            public string DisplayName { get; set; }

            /// <summary>
            /// New password in plain text to be associated with the Person
            /// </summary>
            /// <value></value>
            public string Password { get; set; }
        }

        public class UserDataValidator : AbstractValidator<UserData>
        {
            /// <summary>
            /// Initialize a Create UserDataValidator
            /// </summary>
            public UserDataValidator()
            {
                RuleFor(x => x.Email)
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
            /// <summary>
            /// Instance of the inbound Create UserData
            /// </summary>
            public UserData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            /// <summary>
            /// Initializes a Create CommandValidator
            /// </summary>
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

            /// <summary>
            /// Handles the request 
            /// </summary>
            /// <param name="context">DataContext which the query gets processed on</param>
            /// <param name="passwordHasher">Represents a type used to generate and verify passwords</param>
            /// <param name="jwtTokenGenerator">Represents a type used to generate user specific jwt tokens</param>
            /// <param name="mapper">Represents a type used to do mapping operations using AutoMapper</param>
            /// <param name="mailService">Represents a type used to email send operations</param>
            public Handler(SnippetsContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper, IMailService mailService)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _jwtTokenGenerator = jwtTokenGenerator;
                _mapper = mapper;
                _mailService = mailService;
            }
            
            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                // If the user specifies an email, check whether its already in use
                if (message.User.Email != null && await _context.Persons.Where(u => u.Email == message.User.Email).AnyAsync(cancellationToken))
                    throw RestException.CreateFromDictionary(HttpStatusCode.BadRequest, new Dictionary<string, string> 
                    {
                        {"user.email", $"Email '{ message.User.Email }' is already in use"}
                    });

                // Generate the person object in the data context
                var salt = Guid.NewGuid().ToByteArray();
                                // Generate the person object in the data context
                var person = new Person
                {
                    Email = message.User.Email,
                    DisplayName = message.User.DisplayName,
                    PasswordHash = _passwordHasher.Hash(message.User.Password, salt),
                    PasswordSalt = salt
                };

                // Authenticate the user
                var jwtToken = await _jwtTokenGenerator.CreateToken(person.PersonId);
                var refreshToken =  await _jwtTokenGenerator.CreateRefreshToken(jwtToken);
                person.RefreshToken = refreshToken.Split('.')[2];

                // If there is an email specified, verify it
                if (message.User.Email != null)
                {
                    BackgroundJob.Enqueue(() =>
                        _mailService.SendEmailFromTemplateAsync(message.User.Email, "Welcome to Snippets DB",
                            $"{Directory.GetCurrentDirectory()}/Views/Emails/Registration.cshtml", new
                            {
                                DisplayName = message.User.DisplayName ?? message.User.Email,
                                VerificationUrl = "https://www.youtube.com/watch?v=DLzxrzFCyOs"
                            })
                    );
                }

                _context.Persons.Add(person);
                await _context.SaveChangesAsync(cancellationToken);

                // Map from the data context to a transfer object
                var user = _mapper.Map<Person, User>(person);
                user.Tokens = new UserTokens {
                    Token = jwtToken,
                    Refresh = refreshToken 
                };

                return new UserEnvelope(user);
            }
        }
    }
}
