using HouseBot.Data.Core;

namespace HouseBot.Data.Events
{
    public enum MediaControl
    {
        None,
        Play,
        Pause,
        Stop,
        Next,
        Previous,
        Mute,
    };

    public record MediaControlData(MediaControl Control) : IEventData;
}