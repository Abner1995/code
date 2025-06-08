using ApiUser.Domain.Entities;
using Todo.Domain.Abstractions;

namespace ApiUser.Domain.Events;

public class UserCreatedDomainEvent : IDomainEvent
{
    public User User { get; private set; }

    public UserCreatedDomainEvent(User user)
    {
        this.User = user;
    }
}
