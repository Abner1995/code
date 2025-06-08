using ApiUser.Domain.Repositories;
using DotNetCore.CAP;
using MediatR;

namespace ApiUser.Application.User.Commands.Register;

public class RegisterCommandHandler(IUsersRepository usersRepository, ICapPublisher capPublisher) : IRequestHandler<RegisterCommand>
{
    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new ApiUser.Domain.Entities.User(request.UserName, request.PassWord, null);
        usersRepository.Add(user);
        await usersRepository.UnitOfWork.SaveChangesAsync();
    }
}
