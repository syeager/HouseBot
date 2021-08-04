using HouseBot.Data.Events;
using HouseBot.Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace HouseBot.Server.Controllers
{
    [Route("media")]
    public class MediaControlController : ConsumerController<MediaControlData>
    {
        public MediaControlController(AppData appData)
            : base(appData)
        {
        }
    }
}