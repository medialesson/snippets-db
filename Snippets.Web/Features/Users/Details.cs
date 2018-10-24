using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Exceptions;
using Snippets.Web.Common.Security;
using Snippets.Web.Domains;

namespace Snippets.Web.Features.Users
{
    public class Details
    {
        public class Query : IRequest<UserEnvelope>
        {
            /// <summary>
            /// Initializes a Details Query
            /// </summary>
            /// <param name="userId">Unique identifier for the Person from which the details are retrieved</param>
            public Query(string userId = null)
            {
                UserId = userId;
            }

            /// <summary>
            /// Unique identifier for the Person from which the details are retrieved
            /// </summary>
            public string UserId { get; }
        }

        public class QueryHandler : IRequestHandler<Query, UserEnvelope>
        {
            readonly SnippetsContext _context;
            readonly IJwtTokenGenerator _jwtTokenGenerator;
            readonly ICurrentUserAccessor _currentUserAccessor;
            readonly IMapper _mapper;

            /// <summary>
            /// Initializes a Details QueryHandler 
            /// </summary>
            /// <param name="context">DataContext which the query gets processed on</param>
            /// <param name="jwtTokenGenerator">Represents a type used to generate user specific jwt tokens</param>
            /// <param name="currentUserAccessor">Represents a type used to access the current user from a jwt token</param>
            /// <param name="mapper">Represents a type used to do mapping operations using AutoMapper</param>
            public QueryHandler(SnippetsContext context, IJwtTokenGenerator jwtTokenGenerator, ICurrentUserAccessor currentUserAccessor, IMapper mapper)
            {
                _context = context;
                _jwtTokenGenerator = jwtTokenGenerator;
                _currentUserAccessor = currentUserAccessor;
                _mapper = mapper;
            }

            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<UserEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                // Get the specified user from the database context 
                var person = await _context.Persons.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PersonId == message.UserId, cancellationToken);

                if (message.UserId == null)
                    throw new RedirectException($"user/{_currentUserAccessor.GetCurrentUserId()}", false);

                if (person == null)
                    throw new RestException(HttpStatusCode.NotFound, new Dictionary<string, string>
                    {
                        {"user.id", $"User for id { message.UserId } does not exist"}
                    });

                // Map from the data context to a transfer object
                var user = _mapper.Map<Person, User>(person);

                // Decrease the detail in personalized information if the user is requesting another Person
                if (person.PersonId != _currentUserAccessor.GetCurrentUserId())
                {
                    user.Email = null;
                    user.Tokens = null; // Otherwise you might be able to login as the user LOL
                }
                else
                {
                    var jwtToken = _currentUserAccessor.GetCurrentToken();
                    user.Tokens = new UserTokens 
                    {
                        Token = jwtToken,
                        Refresh = await _jwtTokenGenerator.CreateRefreshToken(jwtToken)
                    };
                }
                return new UserEnvelope(user);
            }
        }
    }
}