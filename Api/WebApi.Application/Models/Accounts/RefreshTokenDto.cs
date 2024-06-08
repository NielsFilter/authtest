namespace WebApi.Models.Accounts;

public class RefreshTokenDto
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReasonRevoked { get; set; }
    public bool IsExpired { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsActive  { get; set; }
}