namespace WebApi.Models.Accounts;

public class AuthenticateDto
{
    public string JwtToken { get; set; }
    //TODO: [JsonIgnore] // refresh token is returned in http only cookie
    public string RefreshToken { get; set; }
    
}