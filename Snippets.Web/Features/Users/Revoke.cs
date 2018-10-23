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
            public string RefreshToken { get; set; } 
        } 

        public class UserTokensDataValidator : AbstractValidator<UserTokensData>
        {
            /// <summary>
            /// Initializes a Revoke UserTokensDataValidator 
            /// </summary>
            public UserTokensDataValidator()
            {
               RuleFor(u => u.RefreshToken).NotEmpty(); 
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
               RuleFor(u => u.Tokens).NotNull().SetValidator(new UserTokensDataValidator()); 
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
                var refreshTokenChecksum = message.Tokens.RefreshToken.Split('.')[2];

                // Get the Person the Refresh token belongs to
                var person = await _context.Persons.Where(x => x.RefreshToken == refreshTokenChecksum).SingleOrDefaultAsync(cancellationToken);

                if (person == null)
                    throw new RestException(HttpStatusCode.BadRequest, "Refresh token does not belong to any user");

                person.RefreshToken = null;

                await _context.SaveChangesAsync();
                throw new RestException(HttpStatusCode.OK, "Refresh token has been revoked");
            }
        }
    }
}