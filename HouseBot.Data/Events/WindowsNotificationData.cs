using HouseBot.Data.Core;

namespace HouseBot.Data.Events
{
    public sealed record WindowsNotificationData(string Message, string BotIcon) : IEventData;
}