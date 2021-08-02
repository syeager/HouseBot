using System.Threading.Tasks;
using HouseBot.Client.Events;
using HouseBot.Data.Core;
using HouseBot.Data.Events;

namespace HouseBot.Client.Consumers
{
    internal sealed class WindowsNotificationConsumer : EventConsumer<WindowsNotificationData>
    {
        public override Task ConsumeAsync(Event<WindowsNotificationData> @event)
        {
            throw new System.NotImplementedException();
        }
    }
}