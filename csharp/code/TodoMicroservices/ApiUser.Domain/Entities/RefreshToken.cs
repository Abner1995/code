using ApiUser.Domain.Events;
using Todo.Domain.Abstractions;

namespace ApiUser.Domain.Entities;

public class RefreshToken : Entity<long>, IAggregateRoot
{
    public string Token { get; set; }
    public DateTime Expiry { get; set; }
    public string DeviceId { get; set; }
    public long UserId { get; set; }

    public RefreshToken()
    {

    }

    public RefreshToken(string token, DateTime expiry, string deviceId, int userId)
    {
        this.Token = token;
        this.Expiry = expiry;
        this.DeviceId = deviceId;
        this.UserId = userId;
        this.AddDomainEvent(new RefreshTokenCreatedDomainEvent(this));
    }
}
