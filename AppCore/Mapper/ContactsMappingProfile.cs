using AutoMapper;
using AppCore.Dto;
using AppCore.Models;
public class ContactsMappingProfile : Profile
{
    public ContactsMappingProfile()
    {
        CreateMap<Person, PersonDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Employer != null ? src.Employer.Name : null));
        
        CreateMap<CreatePersonDto, Person>();
        CreateMap<UpdatePersonDto, Person>();
        CreateMap<Address, AddressDto>().ReverseMap();
    }
}