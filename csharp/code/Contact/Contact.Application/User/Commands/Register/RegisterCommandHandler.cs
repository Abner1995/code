using Contact.Domain.Entities;
using Contact.Domain.Exceptions;
using Contact.Domain.Repositories;
using MediatR;
using UserE = Contact.Domain.Entities.User;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Contact.Application.User.Commands.Register;

public class RegisterCommandHandler(ILogger<RegisterCommandHandler> logger,
    IMapper mapper,
    IUsersRepository usersRepository,
    IPasswordHasher passwordHasher) : IRequestHandler<RegisterCommand>
{
    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(" Register {@User}", request);
        var user = await usersRepository.UserNameExistsAsync(request.UserName);
        if (user)
            throw new FoundException(nameof(UserE), request.UserName);

        request.PassWord = passwordHasher.HashPassword(request.PassWord);
        var ruser = mapper.Map<UserE>(request);
        await usersRepository.AddAsync(ruser);
    }
}
