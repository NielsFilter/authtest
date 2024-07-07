using System.Runtime.InteropServices;

namespace WebApi.Accounts.Models;

using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

public class ProfileUpdateRequest
{
    private string? _password;
    private string? _confirmPassword;
    private string? _email;
    
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [EmailAddress]
    public string? Email
    {
        get => _email;
        set => _email = ReplaceEmptyWithNull(value);
    }

    [MinLength(6)]
    public string? Password
    {
        get => _password;
        set => _password = ReplaceEmptyWithNull(value);
    }

    [Compare("Password")]
    public string? ConfirmPassword 
    {
        get => _confirmPassword;
        set => _confirmPassword = ReplaceEmptyWithNull(value);
    }

    // helpers

    private string? ReplaceEmptyWithNull(string? value)
    {
        // replace empty string with null to make field optional
        return string.IsNullOrEmpty(value) ? null : value;
    }
}