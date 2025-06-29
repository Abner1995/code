namespace ApiUser.Application.Common;

public class LoginResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiryIn { get; set; }

    public LoginResult()
    {

    }

    public LoginResult(string token, string refreshToken, int expiryIn)
    {
        Token = token;
        RefreshToken = refreshToken;
        ExpiryIn = expiryIn;
    }
}
