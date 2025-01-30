using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace Ultimate_WebAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
             .ForMember(c => c.FullAddress,
             opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));
            //CreateMap<Company, CompanyDto>()
            //    .ForCtorParam("FullAddress", // in case of record(..,..,..)
            //        opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));

            CreateMap<Employee, EmployeeDto>();
            CreateMap<CompanyForCreationDto, Company>();
            CreateMap<EmployeeForCreationDto, Employee>();
            CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();
            CreateMap<CompanyForUpdateDto, Company>();


        }
    }
}
