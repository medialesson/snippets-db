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
    public class Revoke
    {
        public class UserTokensData
        {
            /// <summary>
            /// Token associated with the Person that should be revoked
            /// </summary>
            public string Refresh { get; set; } 
        } 

        public class UserTokensDataValidator : AbstractValidator<UserTokensData>
        {
            /// <summary>
            /// Initializes a Revoke UserTokensDataValidator 
            /// </summary>
            public UserTokensDataValidator()
            {
                RuleFor(x => x.Refresh)
                    .NotEmpty().WithMessage("RefreshToken has to have a value")
                    .Matches(@"rft\.[a-zA-z0-9-/].*\.[a-zA-z0-9-/].*").WithMessage("Refresh token value has to match the convention");  
            }
        }

        public class Command : IRequest<object>
        {
            /// <summary>
            /// Instance of the inbound Revoke UserTokensData
            /// </summary>
            public UserTokensData Tokens { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            /// <summary>
            /// Initializes a Revoke CommandValidator
            /// </summary>
            public CommandValidator()
            {
               RuleFor(u => u.Tokens)
                    .NotNull().WithMessage("Payload has to contain a tokens object")
                    .SetValidator(new UserTokensDataValidator()); 
            }
        }

        public class Handler : IRequestHandler<Command, object>
        {
            readonly SnippetsContext _context;
            
            /// <summary>
            /// Handles the request 
            /// </summary>
            /// <param name="context">DataContext which the query gets processed on</param>
            public Handler(SnippetsContext context)
            {
                _context = context;
            }

            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<object> Handle(Command message, CancellationToken cancellationToken)
            {
                var refreshTokenChecksum = message.Tokens.Refresh.Split('.')[2];

                // Get the Person the Refresh token belongs to
                var person = await _context.Persons.Where(x => x.RefreshToken == refreshTokenChecksum).SingleOrDefaultAsync(cancellationToken);

                if (person == null)
                    throw RestException.CreateFromDictionary(HttpStatusCode.BadRequest, new Dictionary<string, string>
                    {
                        {"tokens.refresh", $"Refresh token '{ message.Tokens.Refresh.Substring(0, 12) }...' does not belong to any user"}
                    });

                person.RefreshToken = null;

                await _context.SaveChangesAsync(cancellationToken);
                throw RestException.CreateFromDictionary(HttpStatusCode.OK, new Dictionary<string, string>
                {
                    {"tokens.refresh", $"Refresh token '{ message.Tokens.Refresh.Substring(0, 12) }...' has been revoked"}
                });
            }
        }
    }
}