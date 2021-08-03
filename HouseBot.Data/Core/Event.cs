using System;

namespace HouseBot.Data.Core
{
    public sealed record Event<T>(Guid Id, DateTime Date, string User, T Data) where T : IEventData;
}