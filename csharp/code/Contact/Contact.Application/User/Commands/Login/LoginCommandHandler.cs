using Contact.Application.Common;
using UserE = Contact.Domain.Entities.User;
using Contact.Domain.Exceptions;
using Contact.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Contact.Application.User.Commands.Login;

public class LoginCommandHandler(ILogger<LoginCommandHandler> logger,
    IUsersRepository usersRepository,
    IJwtService jwtService) : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(" Login {@User}", request);
        var user = await usersRepository.GetByUserNameAsync(request.UserName);
        if (user is null)
            throw new NotFoundException(nameof(UserE), request.UserName);

        var token = jwtService.GenerateToken(user);
        return new LoginResult (token, token, 555555);
    }
}
