using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Services;
using WebApi.Services.AppNotifications.Dtos;

namespace WebApi.Helpers;

public class NotificationRepository(DataContext context)
    : BaseRepository<Notification>(context), INotificationRepository
{
    private readonly DataContext _context = context;
    public async Task<IList<Notification>> GetAllByFilterPaged(FilterAccountNotificationsDto input)
    {
        return await _context.Notifications
            .Where(n => n.TargetAccountId == input.AccountId)
            .OrderByDescending(n => n.CreatedBy)
            .Skip(input.Index)
            .Take(input.PageSize)
            .ToListAsync();
    }
}