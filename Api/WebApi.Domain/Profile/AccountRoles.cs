using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Entities;

namespace WebApi.Domain.Profile;

public class AccountRole  : Entity
{
    [ForeignKey(nameof(Account))]
    public int AccountId { get; set; }
    public Account Account { get; set; }
    public Role Role { get; set; }
}