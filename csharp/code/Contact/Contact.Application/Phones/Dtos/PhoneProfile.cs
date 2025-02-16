using AutoMapper;
using Contact.Domain.Entities;

namespace Contact.Application.Phones.Dtos;

public class PhoneProfile:Profile
{
    public PhoneProfile()
    {
        CreateMap<PhoneDto, Phone>();
        CreateMap<Phone, PhoneDto>();
        CreateMap<QueryPhoneDto, Phone>();
        CreateMap<Phone, QueryPhoneDto>();
        CreateMap<UpdatePhoneDto, Phone>();
    }
}
