using HouseBot.Data.Events;
using HouseBot.Data.Services;
using HouseBot.Server.Authorization;

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