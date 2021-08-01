using System.Threading.Tasks;
using HouseBot.Events.Core;

namespace HouseBot.Server.Core
{
    public abstract class EventConsumer<T> where T : IEventData
    {
        public abstract Task ConsumeAsync(Event<T> @event);
    }
}