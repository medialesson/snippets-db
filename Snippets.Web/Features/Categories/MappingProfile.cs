using AutoMapper;

namespace Snippets.Web.Features.Categories
{
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a MappingProfile
        /// </summary>
        public MappingProfile()
        {
            // Create mapping from database context to transfer object
            CreateMap<Domains.Category, Category>();
        }
    }
}
