using WebApi.Entities;

namespace WebApi.Models.Accounts;

public class ListNotificationRequest : PagedRequest
{
    public int AccountId { get; set; }
}