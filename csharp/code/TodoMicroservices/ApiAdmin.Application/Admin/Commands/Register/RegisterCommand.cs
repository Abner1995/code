using MediatR;
using Todo.Core;
using adminEntitie = ApiAdmin.Domain.Entities.Admins;

namespace ApiAdmin.Application.Admin.Commands.Register;

public class RegisterCommand : IRequest<ApiResponse<adminEntitie>>
{
    public string UserName { get; set; }
    public string PassWord { get; set; }
}
