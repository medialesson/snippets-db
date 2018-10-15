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
        public static IQueryable<Snippet> GetAllData(this DbSet<Snippet> snippets)
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
