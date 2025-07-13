using ApiUser.Application.Common;
using MediatR;
using Todo.Core;

namespace ApiUser.Application.User.Commands.Refresh;

public class RefreshTokenCommand : IRequest<ApiResponse<LoginResult>>
{
    public string RefreshToken { get; set; }
    public string DeviceId { get; set; }
}
