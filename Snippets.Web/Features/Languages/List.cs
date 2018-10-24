using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Snippets.Web.Common.Database;
using Snippets.Web.Features.Languages.Enums;

namespace Snippets.Web.Features.Languages
{
    public class List
    {
        public class Query : IRequest<LanguagesEnvelope> { }

        public class QueryHandler : IRequestHandler<Query, LanguagesEnvelope>
        {
            /// <summary>
            /// Handles the request
            /// </summary>
            /// <param name="message">Inbound data from the request</param>
            /// <param name="cancellationToken">CancellationToken to cancel the Task</param>
            public async Task<LanguagesEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                return await Task.Run(() => 
                {
                    var languages = Enum.GetNames(typeof(Language)).ToList();
                    return new LanguagesEnvelope(languages.ConvertAll(i => i.ToLower()).ToArray());
                });
            }
        }
    }
}