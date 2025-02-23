using Contact.Application.Common;
using Contact.Domain.Exceptions;
using Contact.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using UserE = Contact.Domain.Entities.User;

namespace Contact.Application.User.Commands.Login;

public class LoginCommandHandler(ILogger<LoginCommandHandler> logger,
    IUsersRepository usersRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    IJwtService jwtService) : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(" Login {@User}", request);
        var user = await usersRepository.GetByUserNameAsync(request.UserName);
        if (user is null || !passwordHasher.VerifyPassword(user.PassWord, request.PassWord))
            throw new NotFoundException(nameof(UserE), request.UserName);

        var token = jwtService.GenerateToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();
        var tokenExpiry = jwtService.GetTokenExpiry();

        await refreshTokenRepository.CleanUpExpiredRefreshTokens(user.Id);

        if (request.DeviceId != null)
        {
            await refreshTokenRepository.ManageRefreshTokenAsync(user, token, request.DeviceId);
        }

        return new LoginResult(token, token, tokenExpiry);
    }
}