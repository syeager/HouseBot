using System.Threading.Tasks;
using HouseBot.Data.Events;
using HouseBot.Events.Core;
using HouseBot.Server.Core;

namespace HouseBot.Server.Consumers
{
    internal sealed class WindowsNotificationConsumer : EventConsumer<WindowsNotificationData>
    {
        public override Task ConsumeAsync(Event<WindowsNotificationData> @event)
        {
            throw new System.NotImplementedException();
        }
    }
}