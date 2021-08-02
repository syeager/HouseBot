using HouseBot.Data.Events;
using HouseBot.Server.Authorization;
using HouseBot.Server.Services;

namespace HouseBot.Server.Controllers
{
    public class WindowsNotificationController : ConsumerController<WindowsNotificationData>
    {
        public WindowsNotificationController(ApiKeyStore apiKeyStore, GetPartitionIndex getPartitionIndex)
            : base(apiKeyStore, getPartitionIndex)
        {
        }
    }
}