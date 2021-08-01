using HouseBot.Events.Core;

namespace HouseBot.Client.Events
{
    public sealed class EventRequest<T> where T : IEventData
    {
        public string ApiKey { get; set; }
        public string Target { get; set; }
        public T Data { get; set; }
    }
}