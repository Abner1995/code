using ApiUser.Domain.Repositories;
using DotNetCore.CAP;
using MediatR;
using Todo.Core;
using userEntitie = ApiUser.Domain.Entities.User;

namespace ApiUser.Application.User.Commands.Register;

public class RegisterCommandHandler(IUsersRepository usersRepository, ICapPublisher capPublisher) : IRequestHandler<RegisterCommand, ApiResponse<userEntitie>>
{
    public async Task<ApiResponse<userEntitie>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await usersRepository.GetByUserNameAsync(request.UserName, cancellationToken);
        if (existingUser != null)
        {
            return ApiResponse<userEntitie>.Fail(4001, "用户名已存在");
        }
        var user = new userEntitie(request.UserName, request.PassWord, null);
        usersRepository.Add(user);
        await usersRepository.UnitOfWork.SaveChangesAsync();
        return ApiResponse<userEntitie>.Success(user);
    }
}
