using Contact.Application.Common;
using Contact.Domain.Entities;
using Contact.Domain.Exceptions;
using Contact.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Contact.Application.User.Commands.Refresh;

public class RefreshTokenCommandHandler(ILogger<RefreshTokenCommandHandler> logger,
    IRefreshTokenRepository refreshTokenRepository,
    IUsersRepository usersRepository,
    IJwtService jwtService) : IRequestHandler<RefreshTokenCommand, LoginResult>
{
    public async Task<LoginResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(" RefreshToken {@User}", request);
        var storedRefreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

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

        var user = await usersRepository.GetByIdAsync(storedRefreshToken.UserId);
        if (user is null)
            throw new NotFoundException(nameof(RefreshToken), request.RefreshToken);

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
