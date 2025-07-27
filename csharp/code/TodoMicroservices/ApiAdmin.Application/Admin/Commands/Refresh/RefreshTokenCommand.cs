using ApiAdmin.Application.Common;
using MediatR;
using Todo.Core;

namespace ApiAdmin.Application.Admin.Commands.Refresh;

public class RefreshTokenCommand : IRequest<ApiResponse<LoginResult>>
{
    public string RefreshToken { get; set; }
    public string DeviceId { get; set; }
}
