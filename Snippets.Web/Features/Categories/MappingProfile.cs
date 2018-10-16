using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
