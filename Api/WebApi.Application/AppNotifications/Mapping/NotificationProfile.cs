using AutoMapper;
using WebApi.Entities;
using WebApi.Models.Accounts;
using WebApi.Services.AppNotifications.Dtos;

namespace WebApi.Helpers.Mapping;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<NewNotificationRequest, Notification>(); 
        CreateMap<Notification, NewAccountAppNotification>();
        CreateMap<ListNotificationRequest, FilterAccountNotificationsDto>();
    }
}