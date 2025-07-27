using ApiAdmin.Domain.Entities;
using Todo.Domain.Abstractions;

namespace ApiAdmin.Domain.Events;

public class RefreshTokenCreatedDomainEvent : IDomainEvent
{
    public RefreshToken RefreshToken { get; private set; }

    public RefreshTokenCreatedDomainEvent(RefreshToken refreshToken)
    {
        this.RefreshToken = refreshToken;
    }
}
