using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Security;
using Snippets.Web.Common.Services;
using Snippets.Web.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Snippets.Web.Features.Users
{
    public class Edit
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
            /// Initialize a Edit UserDataValidator
            /// </summary>
            public UserDataValidator()
            {
                RuleFor(x => x.Email)
                    .EmailAddress().WithMessage("Email has be a propper email address");
                RuleFor(x => x.Password)
                    .MinimumLength(12).WithMessage("Password has to be at least 12 characters long")
                    .Matches("[A-Z]").WithMessage("Password has to have at least one uppercase letter")
                    .Matches("[a-z]").WithMessage("Password has to have at least one lowercase letter")
                    .Matches("[0-9]").WithMessage("Password has to have at least one number")
                    .Matches("[!\"#$%&'()*+´\\-./:;<=>?@[\\]^_`{|}~]").WithMessage("Password has to have at least one special character");
            }
        } 

        public class Command : IRequest<UserEnvelope>
        {
            /// <summary>
            /// Instance of the inbound Edit UserData
            /// </summary>
            public UserData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            /// <summary>
            /// Initializes a Edit CommandValidator
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
            readonly ICurrentUserAccessor _currentUserAccessor;
            readonly IMapper _mapper;

            /// <summary>
            /// Handles the request 
            /// </summary>
            /// <param name="context">DataContext which the query gets processed on</param>
            /// <param name="passwordHasher">Represents a type used to generate and verify passwords</param>
            /// <param name="currentUserAccessor">Represents a type used to access the current user from a jwt token</param>
            /// <param name="mapper">Represents a type used to do mapping operations using AutoMapper</param>
             public Handler(SnippetsContext context, IPasswordHasher passwordHasher, ICurrentUserAccessor currentUserAccessor, IMapper mapper)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _currentUserAccessor = currentUserAccessor;
                _mapper = mapper;
            }

            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                // Get the current user id from the jwt token and retrieve its Person from the data context
                var currentUserId = _currentUserAccessor.GetCurrentUserId();
                var person = await _context.Persons.Where(p => p.PersonId == currentUserId).FirstOrDefaultAsync(cancellationToken);
                
                // Change the specified properties
                person.Email = message.User.Email ?? person.Email;
                person.DisplayName = message.User.DisplayName ?? person.DisplayName;
                
                if (!string.IsNullOrWhiteSpace(message.User.Password))
                {
                    // Generate a new password hash
                    var salt = Guid.NewGuid().ToByteArray();
                    person.PasswordHash = _passwordHasher.Hash(message.User.Password, salt);
                    person.PasswordSalt = salt;
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Map from the data context to a transfer object
                var user = _mapper.Map<Person, User>(person);
                return new UserEnvelope(user);
            }
        }
    }
}
