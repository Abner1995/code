using MediatR;

namespace ApiUser.Application.User.Commands.Register;

public class RegisterCommand : IRequest
{
    public string UserName { get; set; }
    public string PassWord { get; set; }
}
