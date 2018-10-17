using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippets.Web.Common;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Extensions;
using Snippets.Web.Domains;
using Snippets.Web.Features.Snippets.Enums;

namespace Snippets.Web.Features.Snippets
{
    public class Create
    {
        public class SnippetData
        {
            public string Title { get; set; }
            public string Content { get; set; }
            public Language Language { get; set; }
            public List<string> Categories { get; set; }
        }

        public class SnippetDataValidator : AbstractValidator<SnippetData>
        {
            public SnippetDataValidator()
            {
                RuleFor(x => x.Title).NotEmpty();
                RuleFor(x => x.Content).NotEmpty();
                RuleFor(x => x.Language).NotEmpty();
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
            readonly SnippetsContext _context;
            readonly ICurrentUserAccessor _currentUserAccessor;
            readonly IMapper _mapper;
            readonly AppSettings _settings;

            public Handler(SnippetsContext context, ICurrentUserAccessor currentUserAccessor, IMapper mapper, AppSettings settings)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
                _mapper = mapper;
                _settings = settings;
            }

            public async Task<SnippetEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var author = await _context.Persons.FirstAsync(p => 
                    p.PersonId == _currentUserAccessor.GetCurrentUserId(), 
                    cancellationToken);
                var categories = new List<Category>();

                foreach (var categoryString in message.Snippet.Categories)
                {
                    var category = await _context.Categories.FirstOrDefaultAsync(x => 
                        x.DisplayName.ToLower() == categoryString.ToLower(), 
                        cancellationToken);

                    if (category == null)
                    {
                        category = new Category()
                        {
                            DisplayName = categoryString,
                            Color = _settings.AccentColorsList.Random()
                        };

                        await _context.Categories.AddAsync(category, cancellationToken);
                    }

                    categories.Add(category);
                }

                var newSnippet = new Domains.Snippet()
                {
                    Title = message.Snippet.Title,
                    Author = author,
                    Content = message.Snippet.Content,
                    Language = (int) message.Snippet.Language,
                };
                await _context.Snippets.AddAsync(newSnippet, cancellationToken);

                await _context.SnippetCategories.AddRangeAsync(categories.Select(x => new Domains.SnippetCategory()
                {
                    Snippet = newSnippet,
                    Category = x
                }), cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                var snippet = _mapper.Map<Domains.Snippet, Snippet>(newSnippet);
                return new SnippetEnvelope(snippet);
            }
        }
    }
}
