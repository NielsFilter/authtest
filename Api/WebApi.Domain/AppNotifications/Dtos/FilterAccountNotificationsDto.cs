namespace WebApi.Services.AppNotifications.Dtos;

public class FilterAccountNotificationsDto : FilterPagedDto
{
    public int AccountId { get; set; }
}