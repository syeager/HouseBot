using System;
using HouseBot.Data.Core;

namespace HouseBot.Data.Services
{
    public class GetTopicName
    {
        public string ForEventData(IEventData data) => GetName(data.GetType());
        public string ForType<T>() where T : IEventData => GetName(typeof(T));

        private static string GetName(Type type) => type.Name.ToLower();
    }
}