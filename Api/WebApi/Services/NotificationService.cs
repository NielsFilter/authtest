using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApi.Helpers;
using WebApi.Models.Accounts;

namespace WebApi.Services;

public interface INotificationService
{
    Task<IList<NotificationResponse>> GetAllAccountNotifications(ListNotificationRequest request);
    Task<NotificationResponse> GetAccountNotification(GetNotificationRequest request);
    Task DeleteNotification(DeleteNotificationRequest req);
}

public class NotificationService(
    DataContext context,
    IMapper mapper,
    IOptions<AppSettings> appSettings) : INotificationService
{
    private readonly DataContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<IList<NotificationResponse>> GetAllAccountNotifications(ListNotificationRequest request)
    {
        var notifications = await _context.Notifications
            .Where(n => n.TargetAccountId == request.AccountId)
            .Skip(request.Index)
            .Take(request.PageSize)
            .ToListAsync();
        
        return _mapper.Map<IList<NotificationResponse>>(notifications);
    }

    public async Task<NotificationResponse> GetAccountNotification(GetNotificationRequest request)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n =>
            n.Id == request.NotificationId
            && n.TargetAccountId == request.AccountId);
        return _mapper.Map<NotificationResponse>(notification);
    }

    // create delete notification
    public async Task DeleteNotification(DeleteNotificationRequest req)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n =>
            n.Id == req.Id
            && n.TargetAccountId == req.AccountId
            && n.IsDeleted == false);
        
        if (notification == null) throw new AppException("Notification not found");
        
        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
    }
    
}