using ApiAdmin.Application.Common;
using MediatR;
using Todo.Core;

namespace ApiAdmin.Application.Admin.Commands.Login;

public class LoginCommand : IRequest<ApiResponse<LoginResult>>
{
    public string UserName { get; set; }
    public string PassWord { get; set; }
    public string DeviceId { get; set; }
}
