using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Data.Dto;
using NotificationService.Model.Dto;
using NotificationService.Service.Contract;

namespace NotificationService.Controllers;

[ApiController]
[Authorize(Roles = "GUEST,HOST")]
[Route("api/notification")]
public class NotificationController(INotificationService notificationService) : ControllerBase
{

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<NotificationTypeDto>>> GetAllByRole()
    {
        return Ok(await notificationService.GetAllByRole());
    }

    [HttpGet("typesByUser")]
    public async Task<ActionResult<IEnumerable<NotificationTypeDto>>> GetAllUserCurrent()
    {
        return Ok(await notificationService.GetAllByCurrentUser());
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<NotificationTypeDto>>> Save([FromBody] NotificationTypeDto[] notifications)
    {
        return Ok(await notificationService.Save(notifications));
    }


    [HttpGet("oldNotifications")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> getOldNotifications()
    {
        return Ok(await notificationService.GetOldNotifications());
    }

    
    [HttpPost("updateOldNotifications")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> updateOldNotifications([FromBody] IEnumerable<NotificationDto> notificationDtos)
    {
        return Ok(await notificationService.UpdateOldNotifications(notificationDtos));
    }
}