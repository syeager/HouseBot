using HouseBot.Data.Events;
using HouseBot.Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace HouseBot.Server.Controllers
{
    [Route("windows-note")]
    public class WindowsNotificationController : ConsumerController<WindowsNotificationData>
    {
        public WindowsNotificationController(AppData appData)
            : base(appData)
        {
        }
    }
}