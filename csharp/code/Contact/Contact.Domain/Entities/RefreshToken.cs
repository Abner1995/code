namespace Contact.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime Expiry { get; set; }
    public string DeviceId { get; set; }
    public int UserId { get; set; }
}
