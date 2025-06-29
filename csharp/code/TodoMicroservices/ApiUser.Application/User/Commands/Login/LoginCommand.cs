using ApiUser.Application.Common;
using MediatR;
using Todo.Core;

namespace ApiUser.Application.User.Commands.Login;

public class LoginCommand : IRequest<ApiResponse<LoginResult>>
{
    public string UserName { get; set; }
    public string PassWord { get; set; }
    public string DeviceId { get; set; }
}
