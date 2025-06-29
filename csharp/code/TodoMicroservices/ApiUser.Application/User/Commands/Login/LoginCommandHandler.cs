using ApiUser.Application.Common;
using ApiUser.Domain.Repositories;
using DotNetCore.CAP;
using MediatR;
using Todo.Core;
using Todo.Core.Exceptions;
using UserE = ApiUser.Domain.Entities.User;

namespace ApiUser.Application.User.Commands.Login;

public class LoginCommandHandler(IUsersRepository usersRepository,
    ICapPublisher capPublisher,
    IJwtService jwtService,
    IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<LoginCommand, ApiResponse<LoginResult>>
{
    public async Task<ApiResponse<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByUserNameAsync(request.UserName, cancellationToken);
        if (user is null || user.PassWord != request.PassWord)
        {
            throw new NotFoundException(nameof(UserE), request.UserName);
        }
        var token = jwtService.GenerateToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();
        var tokenExpiry = jwtService.GetTokenExpiry();

        await refreshTokenRepository.CleanUpExpiredRefreshTokens(user.Id, cancellationToken);

        if (request.DeviceId != null)
        {
            await refreshTokenRepository.ManageRefreshTokenAsync(user, token, request.DeviceId, cancellationToken);
        }
        var loginResult = new LoginResult(token, token, tokenExpiry);
        return ApiResponse<LoginResult>.Success(loginResult);
    }
}
