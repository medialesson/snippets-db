using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Security;
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
            public string Email { get; set; }

            public string DisplayName { get; set; }

            public string Password { get; set; }
        }

        public class Command : IRequest<UserEnvelope>
        {
            public UserData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(u => u.User).NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, UserEnvelope>
        {
            readonly SnippetsContext _context;
            readonly IPasswordHasher _passwordHasher;
            readonly ICurrentUserAccessor _currentUserAccessor;
            readonly IMapper _mapper;

            public Handler(SnippetsContext context, IPasswordHasher passwordHasher, ICurrentUserAccessor currentUserAccessor, IMapper mapper)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _currentUserAccessor = currentUserAccessor;
                _mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var currentUserId = _currentUserAccessor.GetCurrentUserId();
                var person = await _context.Persons.Where(p => p.PersonId == currentUserId).FirstOrDefaultAsync(cancellationToken);
                
                person.Email = message.User.Email ?? person.Email;
                person.DisplayName = message.User.DisplayName ?? person.DisplayName;
                
                if (!string.IsNullOrWhiteSpace(message.User.Password))
                {
                    var salt = Guid.NewGuid().ToByteArray();
                    person.PasswordHash = _passwordHasher.Hash(message.User.Password, salt);
                    person.PasswordSalt = salt;
                }

                await _context.SaveChangesAsync(cancellationToken);
                var user = _mapper.Map<Person, User>(person);
                return new UserEnvelope(user);
            }
        }
    }
}
