using AutoMapper;
using WebApi.Domain;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Accounts;
using WebApi.Services.AppNotifications.Dtos;

namespace WebApi.Services;

public interface INotificationService
{
    Task<IList<NewAccountAppNotification>> GetAllAccountNotifications(ListNotificationRequest request);
    Task<NewAccountAppNotification> GetAccountNotification(GetNotificationRequest request);
    Task DeleteNotification(DeleteNotificationRequest req);
    Task NewNotification(NewNotificationRequest newNotification);
}

public class NotificationService(
    IUnitOfWork unitOfWork,
    IAppNotifier appNotifier,
    IMapper mapper) : INotificationService
{
    public async Task<IList<NewAccountAppNotification>> GetAllAccountNotifications(ListNotificationRequest request)
    {
        var input = mapper.Map<FilterAccountNotificationsDto>(request);
        var notifications = await unitOfWork.NotificationRepository.GetAllByFilterPaged(input);
        return mapper.Map<IList<NewAccountAppNotification>>(notifications);
    }

    public async Task<NewAccountAppNotification> GetAccountNotification(GetNotificationRequest request)
    {
        var notification = await unitOfWork.NotificationRepository.GetById(request.NotificationId);
        return mapper.Map<NewAccountAppNotification>(notification);
    }

    public async Task DeleteNotification(DeleteNotificationRequest req)
    {
        var notification = await unitOfWork.NotificationRepository.GetById(req.Id);
        if (notification == null) throw new AppException("Notification not found");
        if(notification.TargetAccountId != req.AccountId) throw new AppException("You don't have permission to delete this notification");
        
        await unitOfWork.NotificationRepository.Delete(req.Id);
        await unitOfWork.Commit();
    }

    public async Task NewNotification(NewNotificationRequest newNotification)
    {
         var notification = mapper.Map<Notification>(newNotification);
         await unitOfWork.NotificationRepository.Insert(notification);
         await unitOfWork.Commit();
         
         await appNotifier.NewAccountNotification(mapper.Map<NewAccountAppNotification>(notification));
    }
}