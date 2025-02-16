using AutoMapper;
using Contact.Application.Contacts.Commands.CreateContact;
using Contact.Application.Contacts.Commands.UpdateContact;
using ContactDomainEntities = Contact.Domain.Entities.Contact;

namespace Contact.Application.Contacts.Dtos;

public class ContactProfile: Profile
{
    public ContactProfile()
    {
        CreateMap<ContactDomainEntities, ContactDto>()
            .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones));
        CreateMap<ContactDto, ContactDomainEntities>()
            .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones));
        CreateMap<CreateContactCommand, ContactDomainEntities>()
            .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones));
        CreateMap<UpdateContactCommand, ContactDomainEntities>()
            .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones));
    }
}
