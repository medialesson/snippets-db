using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Exceptions;
using Snippets.Web.Common.Security;
using Snippets.Web.Domains;

namespace Snippets.Web.Features.Users
{
    public class Auth
    {
        public class UserData
        {
            /// <summary>
            /// Email associated with the Person to authenticate
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// Password in plain text associated with the Person to authenticate 
            /// </summary>
            public string Password { get; set; }
        }

        public class UserDataValidator : AbstractValidator<UserData>
        {
            /// <summary>
            /// Initializes an Auth UserDataValidator
            /// </summary>
            public UserDataValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email has to have a value")
                    .EmailAddress().WithMessage("Email has be a propper email address");
                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password has to have a value");
            }
        }

        public class Command : IRequest<UserEnvelope>
        {
            /// <summary>
            /// Instance of the inbound Auth UserData 
            /// </summary>
            public UserData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            /// <summary>
            /// Initializes an Auth CommandValidator
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

            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                // Check whether there is a User already existing with the same mail associated
                var person = await _context.Persons.Where(p => p.Email == message.User.Email)
                    .SingleOrDefaultAsync(cancellationToken);
                if (person == null)
                    throw new RestException(HttpStatusCode.BadRequest, "Invalid Email");
                
                // Check whether the password is correct
                if (!person.PasswordHash.SequenceEqual(_passwordHasher.Hash(message.User.Password, person.PasswordSalt)))
                    throw new RestException(HttpStatusCode.BadRequest, "Invalid password");

                // Generate tokens and savethe checksum of the refresh token in the data context
                var jwtToken = await _jwtTokenGenerator.CreateToken(person.PersonId);
                var refreshToken =  await _jwtTokenGenerator.CreateRefreshToken(jwtToken);
                person.RefreshToken = refreshToken.Split('.')[2];

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