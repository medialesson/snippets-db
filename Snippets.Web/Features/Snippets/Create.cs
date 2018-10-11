using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common.Database;
using Snippets.Web.Domains;

namespace Snippets.Web.Features.Snippets
{
    public class Create
    {
        public class SnippetData
        {
            public string Title { get; set; }
            public string Content { get; set; }
            public ICollection<string> CategoryList { get; set; }
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
            //private readonly ICurrentUserAccessor _currentUserAccessor;

            public Handler(SnippetsContext context)
            {
                _context = context;
            }

            public async Task<SnippetEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                var author = await _context.Persons.FirstAsync(cancellationToken); // TODO: Assign user in current request
                var categories = new List<Category>();

                foreach (var categoryString in (request.Snippet.CategoryList ?? Enumerable.Empty<string>()))
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
                    Title = request.Snippet.Title,
                    Author = author,
                    Content = request.Snippet.Content
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
