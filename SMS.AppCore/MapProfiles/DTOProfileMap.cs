using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SMS.AppCore.DTOs;
using SMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.MapProfiles
{
    public class DTOProfileMap : Profile
    {
        public DTOProfileMap()
        {
            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(d => d.RoleId, opt => opt.Ignore())
                .ForMember(d => d.RoleName, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<IdentityRole, RoleDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            CreateMap<Class, ClassDTO>().ReverseMap();

        }
    }
}
