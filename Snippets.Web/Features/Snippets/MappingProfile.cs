using AutoMapper;
using Snippets.Web.Domains;

namespace Snippets.Web.Features.Snippets
{
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a MappingProfile
        /// </summary>
        public MappingProfile()
        {
            // Create mapping from database context to transfer object
            CreateMap<Domains.Snippet, Snippet>();
            
            CreateMap<Person, SnippetAuthor>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.PersonId));

            CreateMap<Category, SnippetCategory>();
        }
    }
}