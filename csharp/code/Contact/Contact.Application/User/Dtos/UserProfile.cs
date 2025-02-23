using AutoMapper;
using Contact.Application.User.Commands.Register;
using UserE = Contact.Domain.Entities.User;

namespace Contact.Application.User.Dtos;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterCommand, UserE>();
    }
}
