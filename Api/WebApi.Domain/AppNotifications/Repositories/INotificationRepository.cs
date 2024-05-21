using WebApi.Entities;
using WebApi.Services.AppNotifications.Dtos;

namespace WebApi.Services;

public interface INotificationRepository : IRepository<Notification>
{
    Task<IList<Notification>> GetAllByFilterPaged(FilterAccountNotificationsDto input);
}