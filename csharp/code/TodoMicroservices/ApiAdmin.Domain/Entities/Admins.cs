using ApiAdmin.Domain.Events;
using Todo.Domain.Abstractions;

namespace ApiAdmin.Domain.Entities;

public class Admins : Entity<long>, IAggregateRoot
{
    public string UserName { get; set; }
    public string PassWord { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public Admins()
    {

    }

    public Admins(string userName, string passWord, string? avatar)
    {
        this.UserName = userName;
        this.PassWord = passWord;
        this.Avatar = avatar;
        this.CreatedAt = DateTime.UtcNow;
        this.UpdatedAt = DateTime.UtcNow;
        this.AddDomainEvent(new AdminCreatedDomainEvent(this));
    }
}
