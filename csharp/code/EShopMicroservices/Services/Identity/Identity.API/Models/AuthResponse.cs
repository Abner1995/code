namespace Identity.API.Models;

public class AuthResponse
{
    public string UserId { get; set; } = default!;
    public string Token { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
}
