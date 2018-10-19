using Snippets.Web.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Snippets.Web.Features.Snippets
{
    public static class SnippetExtensions
    {
        /// <summary>
        /// Resolves the dependencies on related databases and returns the data
        /// </summary>
        /// <param name="snippets"></param>
        /// <returns>Snippets dataset with resolved relationships</returns>
        public static IQueryable<Domains.Snippet> GetAllData(this DbSet<Domains.Snippet> snippets)
        {
            return snippets
                .Include(x => x.Author)
                .Include(x => x.SnippetCategories).ThenInclude(x => x.Snippet)
                .Include(x => x.SnippetCategories).ThenInclude(x => x.Category)
                .Include(x => x.Karma).ThenInclude(x => x.Snippet)
                .Include(x => x.Karma).ThenInclude(x => x.Submitter)
                .AsNoTracking(); // Do not flag any changes as dirty
        }
    }
}
