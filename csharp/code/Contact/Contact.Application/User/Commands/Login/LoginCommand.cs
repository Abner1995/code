using Contact.Application.Common;
using MediatR;

namespace Contact.Application.User.Commands.Login;

public class LoginCommand: IRequest<LoginResult>
{
    public string UserName { get; set; }
    public string PassWord { get; set; }
}
