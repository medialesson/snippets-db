using AutoMapper;
using Snippets.Web.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snippets.Web.Features.Users
{
    /// <summary>
    /// Initializes a MappingProfile
    /// </summary>
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            // Create mapping from database context to transfer object
            CreateMap<Person, User>(MemberList.None)
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.PersonId));
        }
    }
}
