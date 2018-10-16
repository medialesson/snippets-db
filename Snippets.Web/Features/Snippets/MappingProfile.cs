using AutoMapper;
using Snippets.Web.Domains;

namespace Snippets.Web.Features.Snippets
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domains.Snippet, Snippet>();
            
            CreateMap<Person, SnippetAuthor>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.PersonId));

            CreateMap<Category, SnippetCategory>();
        }
    }
}