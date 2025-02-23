using Contact.Application.Common;
using MediatR;

namespace Contact.Application.User.Commands.Refresh;

public class RefreshTokenCommand : IRequest<LoginResult>
{
    public string RefreshToken { get; set; }
    public string DeviceId { get; set; }
}
