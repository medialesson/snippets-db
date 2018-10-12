using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common;
using Snippets.Web.Common.Database;
using Snippets.Web.Domains;
using Snippets.Web.Domains.Enums;

namespace Snippets.Web.Features.Snippets
{
    public class Create
    {
        public class SnippetData
        {
            public string Title { get; set; }
            public string Content { get; set; }
            public ProgrammingLanguage ProgrammingLanguage { get; set; }
            public ICollection<string> Categories { get; set; }
        }

        public class SnippetDataValidator : AbstractValidator<SnippetData>
        {
            public SnippetDataValidator()
            {
                RuleFor(x => x.Title).NotNull().NotEmpty();
                RuleFor(x => x.Content).NotNull().NotEmpty();
            }
        }

        public class Command : IRequest<SnippetEnvelope>
        {
            public SnippetData Snippet { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Snippet).NotNull().SetValidator(new SnippetDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, SnippetEnvelope>
        {
            private readonly SnippetsContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public Handler(SnippetsContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<SnippetEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var author = await _context.Persons.FirstAsync(p => p.PersonId == _currentUserAccessor.GetCurrentUserId(), cancellationToken);
                var categories = new List<Category>();

                foreach (var categoryString in (message.Snippet.Categories ?? Enumerable.Empty<string>()))
                {
                    var c = await _context.Categories.FirstOrDefaultAsync(x => 
                        x.DisplayName.ToLower() == categoryString.ToLower(), 
                        cancellationToken);

                    if (c == null)
                    {
                        c = new Category()
                        {
                            DisplayName = categoryString
                        };

                        await _context.Categories.AddAsync(c, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    categories.Add(c);
                }

                var snippet = new Snippet()
                {
                    Title = message.Snippet.Title,
                    Author = author,
                    Content = message.Snippet.Content,
                    ProgrammingLanguage = message.Snippet.ProgrammingLanguage
                };

                await _context.Snippets.AddAsync(snippet, cancellationToken);

                await _context.SnippetCategories.AddRangeAsync(categories.Select(x => new SnippetCategory()
                {
                    Snippet = snippet,
                    Category = x
                }), cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return new SnippetEnvelope(snippet);
            }
        }
    }
}
