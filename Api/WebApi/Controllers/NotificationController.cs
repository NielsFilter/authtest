using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Accounts.Models;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route($"{ApiVersioning.V1}/[controller]")]
public class NotificationController(
    ITokenAuthService tokenAuthService,
    INotificationService notificationService) : AuthenticatedController(tokenAuthService)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewAccountAppNotification>>> GetAll([FromQuery] PagedRequest request)
    {
        await notificationService.NewNotification(new NewNotificationRequest()
        {
            Message = "Hello world " + DateTime.Now.ToString(CultureInfo.InvariantCulture),
            Type = NotificationTypes.Info,
            TargetAccountId = AccountId!.Value
        });
         
        var notifications = await notificationService.GetAllAccountNotifications(new ListNotificationRequest
        {
            AccountId = AccountId!.Value,
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
            AccountId = AccountId!.Value,
            NotificationId = id
        });
        
        if (notification == null)
        {
            return NotFound(id);
        }
        
        return Ok(notification);
    }
    
    [HttpDelete]
    public async Task<ActionResult> DeleteNotification(int notificationId)
    {
        await notificationService.DeleteNotification(new DeleteNotificationRequest()
        {
            AccountId = AccountId!.Value,
            Id = notificationId
        });
        
        return NoContent();
    }
}