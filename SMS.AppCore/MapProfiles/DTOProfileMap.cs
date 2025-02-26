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

            CreateMap<Student, StudentDTO>()
                .ForMember(dest => dest.ClassName, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Class, ClassDTO>()
                .ForMember(dest => dest.Subjects, opt => opt.Ignore())
                .ReverseMap();


            CreateMap<Subject, SubjectDTO>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.SubjectName))
                .ReverseMap();

            CreateMap<Exam, ExamDTO>().ReverseMap();

            CreateMap<TeacherClassSubject, TeacherDTO>().ReverseMap();

            CreateMap<ExamInitiation, ExamInitiationDTO>().ReverseMap();

            CreateMap<ClassSubjectMarksDTO, ClassSubjectMarksViewModel>().ReverseMap();

        }
    }
}
