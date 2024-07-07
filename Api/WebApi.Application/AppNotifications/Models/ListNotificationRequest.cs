using WebApi.Models;

namespace WebApi.Accounts.Models;

public class ListNotificationRequest : PagedRequest
{
    public int AccountId { get; set; }
}