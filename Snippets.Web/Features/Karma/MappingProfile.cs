using AutoMapper;

namespace Snippets.Web.Features.Karma
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domains.Karma, Vote>(MemberList.None)
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Upvote));
        }
    }
}
    