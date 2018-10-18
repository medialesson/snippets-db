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
            /// <summary>
            /// Email to associate with the Person
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// Name that gets displayed to other users to associate with the Person
            /// </summary>
            public string DisplayName { get; set; }

            /// <summary>
            /// Password in plain text to associate with the Person
            /// </summary>
            public string Password { get; set; }
        }

        public class UserDataValidator : AbstractValidator<UserData>
        {
            /// <summary>
            /// Initialize a Create UserDataValidator
            /// </summary>
            public UserDataValidator()
            {
                RuleFor(d => d.Email).NotEmpty().EmailAddress();
                RuleFor(d => d.Password).NotEmpty(); 
                // TODO: Implement the use of save passwords only
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
                RuleFor(c => c.User).NotNull().SetValidator(new UserDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, UserEnvelope>
        {
            readonly SnippetsContext _context;
            readonly IPasswordHasher _passwordHasher;
            readonly IJwtTokenGenerator _jwtTokenGenerator;
            readonly IMapper _mapper;

            /// <summary>
            /// Handles the request 
            /// </summary>
            /// <param name="context">DataContext which the query gets processed on</param>
            /// <param name="passwordHasher">Represents a type used to generate and verify passwords</param>
            /// <param name="jwtTokenGenerator">Represents a type used to generate user specific jwt tokens</param>
            /// <param name="mapper">Represents a type used to do mapping operations using AutoMapper</param>
            public Handler(SnippetsContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _jwtTokenGenerator = jwtTokenGenerator;
                _mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                // Check whether there is a User already existing with the same mail associated
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

                // Map from the data context to a transfer object
                var user = _mapper.Map<Person, User>(person);
                user.Token = await _jwtTokenGenerator.CreateToken(person.PersonId);
                return new UserEnvelope(user);
            }
        }
    }
}
