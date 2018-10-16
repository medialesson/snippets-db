using System;
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
            public Query(string userId = null)
            {
                UserId = userId;
            }
            public string UserId { get; }
        }

        public class QueryHandler : IRequestHandler<Query, UserEnvelope>
        {
            readonly SnippetsContext _context;
            readonly IJwtTokenGenerator _jwtTokenGenerator;
            readonly ICurrentUserAccessor _currentUserAccessor;
            readonly IMapper _mapper;

            public QueryHandler(SnippetsContext context, IJwtTokenGenerator jwtTokenGenerator, ICurrentUserAccessor currentUserAccessor, IMapper mapper)
            {
                _context = context;
                _jwtTokenGenerator = jwtTokenGenerator;
                _currentUserAccessor = currentUserAccessor;
                _mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var person = await _context.Persons.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PersonId == message.UserId, cancellationToken);

                if (message.UserId == null)
                    throw new RedirectException($"user/{_currentUserAccessor.GetCurrentUserId()}", false);

                if (person == null)
                    throw new RestException(HttpStatusCode.NotFound, "User does not exist");

                var user = _mapper.Map<Person, User>(person);

                if (person.PersonId != _currentUserAccessor.GetCurrentUserId())
                {
                    user.Email = null;
                    user.Token = null;
                }
                else
                    user.Token = await _jwtTokenGenerator.CreateToken(person.PersonId);
                return new UserEnvelope(user);
            }
        }
    }
}