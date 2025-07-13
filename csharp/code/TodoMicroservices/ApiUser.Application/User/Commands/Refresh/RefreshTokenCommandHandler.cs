using ApiUser.Application.Common;
using ApiUser.Domain.Entities;
using ApiUser.Domain.Repositories;
using MediatR;
using Todo.Core;
using Todo.Core.Exceptions;

namespace ApiUser.Application.User.Commands.Refresh;

public class RefreshTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository,
    IUsersRepository usersRepository,
    IJwtService jwtService) : IRequestHandler<RefreshTokenCommand, ApiResponse<LoginResult>>
{
    public async Task<ApiResponse<LoginResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var storedRefreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (storedRefreshToken is null)
            throw new NotFoundException(nameof(RefreshToken), request.RefreshToken);

        var expiryTime = storedRefreshToken.Expiry; // 假设数据库中的时间是UTC时间
        // 确保expiryTime是UTC时间
        if (expiryTime.Kind != DateTimeKind.Utc)
        {
            expiryTime = TimeZoneInfo.ConvertTimeToUtc(expiryTime);
        }

        var expiryDateTimeOffset = new DateTimeOffset(expiryTime);
        var currentUtcDateTimeOffset = DateTimeOffset.UtcNow;

        if (expiryDateTimeOffset <= currentUtcDateTimeOffset)
            throw new NotFoundException(nameof(RefreshToken), request.RefreshToken);

        var user = await usersRepository.GetAsync(storedRefreshToken.UserId);
        if (user is null)
            throw new NotFoundException(nameof(RefreshToken), request.RefreshToken);

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
