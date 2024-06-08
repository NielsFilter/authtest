using AutoMapper;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Accounts;
using WebApi.Services.AppNotifications.Dtos;

namespace WebApi.Services;

public interface INotificationService
{
    Task<IList<NewAccountAppNotification>> GetAllAccountNotifications(ListNotificationRequest request);
    Task<NewAccountAppNotification?> GetAccountNotification(GetNotificationRequest request);
    Task DeleteNotification(DeleteNotificationRequest req);
    Task NewNotification(NewNotificationRequest newNotification);
}

public class NotificationService(
    INotificationRepository notificationRepository,
    IAppNotifier appNotifier,
    IMapper mapper) : INotificationService
{
    public async Task<IList<NewAccountAppNotification>> GetAllAccountNotifications(ListNotificationRequest request)
    {
        var input = mapper.Map<FilterAccountNotificationsDto>(request);
        var notifications = await notificationRepository.GetAllByFilterPaged(input);
        return mapper.Map<IList<NewAccountAppNotification>>(notifications);
    }

    public async Task<NewAccountAppNotification?> GetAccountNotification(GetNotificationRequest request)
    {
        var notification = await notificationRepository.GetById(request.NotificationId);
        return mapper.Map<NewAccountAppNotification>(notification);
    }

    public async Task DeleteNotification(DeleteNotificationRequest req)
    {
        var notification = await notificationRepository.GetById(req.Id);
        if (notification == null) throw new AppException("Notification not found");
        if(notification.TargetAccountId != req.AccountId) throw new AppException("You don't have permission to delete this notification");
        
        await notificationRepository.Delete(req.Id);
    }

    public async Task NewNotification(NewNotificationRequest newNotification)
    {
         var notification = mapper.Map<Notification>(newNotification);
         await notificationRepository.Insert(notification);
         
         await appNotifier.NewAccountNotification(mapper.Map<NewAccountAppNotification>(notification));
    }
}