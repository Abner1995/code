using ApiAdmin.Domain.Entities;
using Todo.Domain.Abstractions;

namespace ApiAdmin.Domain.Events;

public class AdminCreatedDomainEvent : IDomainEvent
{
    public Admins Admins { get; private set; }

    public AdminCreatedDomainEvent(Admins admins)
    {
        this.Admins = admins;
    }
}
