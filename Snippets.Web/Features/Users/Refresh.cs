using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Exceptions;
using Snippets.Web.Common.Security;

namespace Snippets.Web.Features.Users
{
    public class Refresh
    {
        public class UserTokensData
        {
            /// <summary>
            /// Token associated with the Person to refresh the Jwt token
            /// </summary>
            public string RefreshToken { get; set; } 
        } 

        public class UserTokensDataValidator : AbstractValidator<UserTokensData>
        {
            /// <summary>
            /// Initializes a Refresh UserTokensDataValidator 
            /// </summary>
            public UserTokensDataValidator()
            {
               RuleFor(u => u.RefreshToken).NotEmpty(); 
            }
        }

        public class Command : IRequest<UserTokensEnvelope>
        {
            /// <summary>
            /// Instance of the inbound Refresh UserTokensData
            /// </summary>
            public UserTokensData Tokens { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            /// <summary>
            /// Initializes a Refresh CommandValidator
            /// </summary>
            public CommandValidator()
            {
               RuleFor(u => u.Tokens).NotNull().SetValidator(new UserTokensDataValidator()); 
            }
        }

        public class Handler : IRequestHandler<Command, UserTokensEnvelope>
        {
            readonly SnippetsContext _context;
            readonly IJwtTokenGenerator _jwtTokenGenerator;
            readonly ICurrentUserAccessor _currentUserAccessor;
            
            /// <summary>
            /// Handles the request 
            /// </summary>
            /// <param name="context">DataContext which the query gets processed on</param>
            /// <param name="jwtTokenGenerator">Represents a type used to generate user specific jwt tokens</param>
            /// <param name="currentUserAccessor">Represents a type used to access the current user from a jwt token</param>
            public Handler(SnippetsContext context, IJwtTokenGenerator jwtTokenGenerator, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _jwtTokenGenerator = jwtTokenGenerator;
                _currentUserAccessor = currentUserAccessor;
            }

            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<UserTokensEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var jwtToken = _currentUserAccessor.GetCurrentToken();
                var refreshToken = message.Tokens.RefreshToken;
                var refreshTokenChecksum = refreshToken.Split('.')[2];

                // Get the Person the Refresh token belongs to
                var person = await _context.Persons.Where(x => x.RefreshToken == refreshTokenChecksum).SingleOrDefaultAsync(cancellationToken);

                if (person == null)
                    throw new RestException(HttpStatusCode.BadRequest, "Refresh token does not belong to any user");

                // Validate the Refresh token
                if (!await _jwtTokenGenerator.VerifyRefreshToken(refreshToken, jwtToken))
                    throw new RestException(HttpStatusCode.BadRequest, "Refresh token is invalid");

                // Generate a new Jwt and Refresh token
                var newJwtToken = await _jwtTokenGenerator.CreateToken(person.PersonId);
                var newRefreshToken = await _jwtTokenGenerator.CreateRefreshToken(newJwtToken);
                person.RefreshToken = newRefreshToken.Split('.')[2];

                await _context.SaveChangesAsync();

                return new UserTokensEnvelope(new UserTokens
                {
                    Token = newJwtToken,
                    RefreshToken = newRefreshToken
                });
            }
        }
    }
}