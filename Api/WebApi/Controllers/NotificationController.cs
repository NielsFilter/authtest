using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.Models;
using WebApi.Models.Accounts;
using WebApi.Services;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class NotificationController : BaseController
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<NotificationResponse>> GetAll(PagedRequest request)
    {
        var notifications = _notificationService.GetAllAccountNotifications(new ListNotificationRequest
        {
            AccountId = Account.Id,
            Index = request.Index,
            PageSize = request.PageSize
        });
        return Ok(notifications);
    }
    
    [HttpGet]
    public ActionResult<NotificationResponse> GetAll(int notificationId)
    {
        var notification = _notificationService.GetAccountNotification(new GetNotificationRequest
        {
            AccountId = Account.Id,
            NotificationId = notificationId
        });
        if (notification == null)
        {
            return NotFound();
        }
        return Ok(notification);
    }
    
    [HttpDelete]
    public ActionResult DeleteNotification(int notificationId)
    {
        _notificationService.DeleteNotification(new DeleteNotificationRequest()
        {
            AccountId = Account.Id,
            Id = notificationId
        });
        return NoContent();
    }
}