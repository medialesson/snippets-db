using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Snippets.Web.Common;
using Snippets.Web.Common.Database;
using Snippets.Web.Common.Exceptions;
using Snippets.Web.Common.Extensions;
using Snippets.Web.Common.Services;
using Snippets.Web.Domains;
using Snippets.Web.Features.Languages.Enums;

namespace Snippets.Web.Features.Snippets
{
    public class Edit
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
            /// Programming language the Snippets content is in as string
            /// </summary>
            public string Language { get; set; }

            /// <summary>
            /// List of Category names the Snippet should be attached to
            /// </summary>
            public List<string> Categories { get; set; } 
        }

        public class Command : IRequest<SnippetEnvelope>
        {
            public SnippetData Snippet { get; set; }

            [JsonProperty("id")]
            [JsonIgnore]
            public string SnippetId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            /// <summary>
            /// Initializes a Create CommandValidator
            /// </summary>
            public CommandValidator()
            {
                RuleFor(x => x.Snippet).NotNull().WithMessage("Payload has to contain a snippet object");
            }
        }
        
        public class Handler : IRequestHandler<Command, SnippetEnvelope>
        {
            readonly SnippetsContext _context;
            readonly ICurrentUserAccessor _currentUserAccessor;

            readonly AppSettings _appSettings;

            readonly IMapper _mapper;

            /// <summary>
            /// Initializes a Create Handler
            /// </summary>
            /// <param name="context">DataContext which the query gets processed on</param>
            /// <param name="currentUserAccessor">Represents a type used to access the current user from a jwt token</param>
            /// <param name="appSettings">Mapper for the "appsettings.json" file</param>
            /// <param name="mapper">Represents a type used to do mapping operations using AutoMapper</param>
            public Handler(SnippetsContext context, ICurrentUserAccessor currentUserAccessor, AppSettings appSettings, IMapper mapper)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
                _appSettings = appSettings;
                _mapper = mapper;
            }

            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<SnippetEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(message.SnippetId))
                    throw RestException.CreateFromDictionary(HttpStatusCode.NotFound, new Dictionary<string, string>
                    {
                        {"snippet.id", $"Id has to have a value"}
                    });

                var author = await _context.Persons.SingleOrDefaultAsync(p => 
                    p.PersonId == _currentUserAccessor.GetCurrentUserId(), 
                    cancellationToken);
                var selectedSnippet = await _context.Snippets.SingleOrDefaultAsync(x => x.SnippetId == message.SnippetId);;

                if (selectedSnippet != null)
                {                    
                    if (selectedSnippet.Author.PersonId != author.PersonId)
                        throw RestException.CreateFromDictionary(HttpStatusCode.NotFound, new Dictionary<string, string>
                        {
                            {"snippet", $"User for id '{ author.PersonId }' is not the author of Snippet for id '{ selectedSnippet.SnippetId }'"}
                        });

                    _context.Snippets.Remove(selectedSnippet);
                    
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
                                Color = _appSettings.AccentColorsList.Random()
                            };

                            await _context.Categories.AddAsync(category, cancellationToken);
                        }

                        categories.Add(category);
                    }

                    if (!string.IsNullOrEmpty(message.Snippet.Language))
                    {
                        // Resolve language enum from string
                        if (!Enum.TryParse(message.Snippet.Language, true, out Language language))
                            throw RestException.CreateFromDictionary(HttpStatusCode.BadRequest, new Dictionary<string, string>
                            {
                                {"snippet.language", $"Language '{ message.Snippet.Language }' is not registered"}
                            });
                        
                        selectedSnippet.Language = (int) language;
                    }

                    var newSnippet = new Domains.Snippet()
                    {
                        SnippetId = selectedSnippet.SnippetId,
                        Title = message.Snippet.Title ?? selectedSnippet.Content,
                        Content = message.Snippet.Content ?? selectedSnippet.Content,
                        Author = selectedSnippet.Author,
                        Language = selectedSnippet.Language,
                    };
                    await _context.Snippets.AddAsync(newSnippet, cancellationToken);

                    await _context.SnippetCategories.AddRangeAsync(categories.Select(x => new Domains.SnippetCategory()
                    {
                        Snippet = newSnippet,
                        Category = x
                    }), cancellationToken); 

                    await _context.Snippets.AddAsync(newSnippet);

                    // Map from the data context to a transfer object
                    var snippet = _mapper.Map<Domains.Snippet, Snippet>(newSnippet);
                    return new SnippetEnvelope(snippet);
                }
                else
                {
                    throw RestException.CreateFromDictionary(HttpStatusCode.NotFound, new Dictionary<string, string>
                    {
                        {"snippet.id", $"Snippet for id '{ message.SnippetId }' does not exist"}
                    });
                }
            }
        }
    }
}
