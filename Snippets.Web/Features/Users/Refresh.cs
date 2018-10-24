using System.Collections.Generic;
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
            public string Refresh { get; set; } 
        } 

        public class UserTokensDataValidator : AbstractValidator<UserTokensData>
        {
            /// <summary>
            /// Initializes a Refresh UserTokensDataValidator 
            /// </summary>
            public UserTokensDataValidator()
            {
                RuleFor(x => x.Refresh)
                    .NotEmpty().WithMessage("RefreshToken has to have a value")
                    .Matches(@"rft\.[A-z0-9].*\.[A-z0-9].*").WithMessage("Refresh token value has to match the convention"); 
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
                RuleFor(x => x.Tokens)
                    .NotNull().WithMessage("Payload has to contain a tokens object")
                    .SetValidator(new UserTokensDataValidator()); 
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
                var refreshToken = message.Tokens.Refresh;
                var refreshTokenChecksum = refreshToken.Split('.')[2];

                // Get the Person the Refresh token belongs to
                var person = await _context.Persons.Where(x => x.RefreshToken == refreshTokenChecksum).SingleOrDefaultAsync(cancellationToken);

                if (person == null)
                    throw RestException.CreateFromDictionary(HttpStatusCode.BadGateway, new Dictionary<string, string>
                    {
                        {"tokens.refresh", $"Refresh token '{ refreshToken.Substring(0, 12) }...' does not belong to any user"}
                    });

                // Validate the Refresh token
                if (!await _jwtTokenGenerator.VerifyRefreshToken(refreshToken, jwtToken))
                    throw RestException.CreateFromDictionary(HttpStatusCode.BadRequest, new Dictionary<string, string>
                    {
                        {"tokens.refresh", $"Refresh token '{ refreshToken.Substring(0, 12) }...' is invalid"}
                    });

                // Generate a new Jwt and Refresh token
                var newJwtToken = await _jwtTokenGenerator.CreateToken(person.PersonId);
                var newRefreshToken = await _jwtTokenGenerator.CreateRefreshToken(newJwtToken);
                person.RefreshToken = newRefreshToken.Split('.')[2];

                await _context.SaveChangesAsync(cancellationToken);

                return new UserTokensEnvelope(new UserTokens
                {
                    Token = newJwtToken,
                    Refresh = newRefreshToken
                });
            }
        }
    }
}