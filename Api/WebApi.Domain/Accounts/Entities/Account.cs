using WebApi.Domain.Settings;

namespace WebApi.Entities;

public class Account : Entity
{
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? Image { get; set; }
    public string PasswordHash { get; set; }
    public bool AcceptTerms { get; set; }
    public string? VerificationToken { get; set; }
    public DateTime? Verified { get; set; }
    public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
    public DateTime? PasswordReset { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? Updated { get; set; }

    public ProfileSetting? Settings { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}