namespace Contact.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PassWord { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
