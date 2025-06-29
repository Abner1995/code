using MediatR;
using Todo.Core;
using userEntitie = ApiUser.Domain.Entities.User;

namespace ApiUser.Application.User.Commands.Register;

public class RegisterCommand : IRequest<ApiResponse<userEntitie>>
{
    public string UserName { get; set; }
    public string PassWord { get; set; }
}
