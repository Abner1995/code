using ApiUser.Domain.Entities;
using Todo.Domain.Abstractions;

namespace ApiUser.Domain.Events;

public class RefreshTokenCreatedDomainEvent : IDomainEvent
{
    public RefreshToken RefreshToken { get; private set; }

    public RefreshTokenCreatedDomainEvent(RefreshToken refreshToken)
    {
        this.RefreshToken = refreshToken;
    }
}
