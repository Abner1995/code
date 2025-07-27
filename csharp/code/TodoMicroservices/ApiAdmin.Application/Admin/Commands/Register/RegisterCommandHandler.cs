using ApiAdmin.Domain.Repositories;
using DotNetCore.CAP;
using MediatR;
using Todo.Core;
using adminEntitie = ApiAdmin.Domain.Entities.Admins;

namespace ApiAdmin.Application.Admin.Commands.Register;

public class RegisterCommandHandler(IAdminsRepository adminsRepository, ICapPublisher capPublisher) : IRequestHandler<RegisterCommand, ApiResponse<adminEntitie>>
{
    public async Task<ApiResponse<adminEntitie>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingAdmin = await adminsRepository.GetByUserNameAsync(request.UserName, cancellationToken);
        if (existingAdmin != null)
        {
            return ApiResponse<adminEntitie>.Fail(4001, "用户名已存在");
        }
        var admin = new adminEntitie(request.UserName, request.PassWord, null);
        adminsRepository.Add(admin);
        await adminsRepository.UnitOfWork.SaveChangesAsync();
        return ApiResponse<adminEntitie>.Success(admin);
    }
}
