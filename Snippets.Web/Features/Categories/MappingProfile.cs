using AutoMapper;

namespace Snippets.Web.Features.Categories
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domains.Category, Category>();
        }
    }
}
