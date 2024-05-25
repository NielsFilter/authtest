using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Models;
using WebApi.Models.Accounts;
using WebApi.Services;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class NotificationController(INotificationService notificationService) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewAccountAppNotification>>> GetAll([FromQuery] PagedRequest request)
    {
        await notificationService.NewNotification(new NewNotificationRequest()
        {
            Message = "Hello world " + DateTime.Now.ToString(CultureInfo.InvariantCulture),
            Type = NotificationTypes.Info,
            TargetAccountId = this.Account.Id
        });
         
        var notifications = await notificationService.GetAllAccountNotifications(new ListNotificationRequest
        {
            AccountId = Account.Id,
            Index = request.Index,
            PageSize = request.PageSize
        });
        return Ok(notifications);
    }
    
   
    [HttpGet("{id:int}")]
    public async Task<ActionResult<NewAccountAppNotification>> GetById(int id)
    {
        var notification = await notificationService.GetAccountNotification(new GetNotificationRequest
        {
            AccountId = Account.Id,
            NotificationId = id
        });
        if (notification == null)
        {
            return NotFound();
        }
        return Ok(notification);
    }
    
    [HttpDelete]
    public async Task<ActionResult> DeleteNotification(int notificationId)
    {
        await notificationService.DeleteNotification(new DeleteNotificationRequest()
        {
            AccountId = Account.Id,
            Id = notificationId
        });
        return NoContent();
    }
}