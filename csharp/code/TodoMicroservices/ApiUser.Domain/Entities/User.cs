using ApiUser.Domain.Events;
using Todo.Domain.Abstractions;

namespace ApiUser.Domain.Entities;

public class User : Entity<long>, IAggregateRoot
{
    public string UserName { get; set; }
    public string PassWord { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public User()
    {

    }

    public User(string userName, string passWord, string? avatar)
    {
        this.UserName = userName;
        this.PassWord = passWord;
        this.Avatar = avatar;
        this.AddDomainEvent(new UserCreatedDomainEvent(this));
    }
}
