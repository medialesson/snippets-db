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
using Snippets.Web.Common.Services;
using Snippets.Web.Domains;
using Snippets.Web.Features.Snippets.Enums;

namespace Snippets.Web.Features.Snippets
{
    public class Create
    {
        public class SnippetData
        {
            /// <summary>
            /// Title of the Snippet 
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Content of the Snippet (usually code)
            /// </summary>
            public string Content { get; set; }

            /// <summary>
            /// Programming language the Snippets content is in
            /// </summary>
            public Language Language { get; set; }

            /// <summary>
            /// List of Category names the Snippet should be attached to
            /// </summary>
            public List<string> Categories { get; set; }
        }

        public class SnippetDataValidator : AbstractValidator<SnippetData>
        {
            /// <summary>
            /// Initializes a Create SnippetDataValidator
            /// </summary>
            public SnippetDataValidator()
            {
                RuleFor(x => x.Title).NotEmpty().WithMessage("Title has to have a value");
                RuleFor(x => x.Content).NotEmpty().WithMessage("Content has to have a value");
                RuleFor(x => x.Language)
                    .NotEmpty().WithMessage("Language has to have a value")
                    .IsInEnum().WithMessage("Language value has to be a Language enum");
            }
        }

        public class Command : IRequest<SnippetEnvelope>
        {
            /// <summary>
            /// Instance of the inbound Create SnippetData
            /// </summary>
            public SnippetData Snippet { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            /// <summary>
            /// Initializes a Create CommandValidator
            /// </summary>
            public CommandValidator()
            {
                RuleFor(x => x.Snippet)
                    .NotNull().WithMessage("Payload has to contain a snippet object")
                    .SetValidator(new SnippetDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, SnippetEnvelope>
        {
            readonly SnippetsContext _context;
            readonly ICurrentUserAccessor _currentUserAccessor;
            readonly IMapper _mapper;
            readonly AppSettings _settings;

            /// <summary>
            /// Initializes a Create Handler
            /// </summary>
            /// <param name="context">DataContext which the query gets processed on</param>
            /// <param name="currentUserAccessor">Represents a type used to access the current user from a jwt token</param>
            /// <param name="mapper">Represents a type used to do mapping operations using AutoMapper</param>
            /// <param name="settings">Mapper for the appsettings.json file</param>
            public Handler(SnippetsContext context, ICurrentUserAccessor currentUserAccessor, IMapper mapper, AppSettings settings)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
                _mapper = mapper;
                _settings = settings;
            }

            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
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

                // Map from the data context to a transfer object
                var snippet = _mapper.Map<Domains.Snippet, Snippet>(newSnippet);
                return new SnippetEnvelope(snippet);
            }
        }
    }
}
