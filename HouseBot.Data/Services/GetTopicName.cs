using System;
using HouseBot.Data.Core;

namespace HouseBot.Data.Services
{
    public class GetTopicName
    {
        public string ForEventData(IEventData data) => ForType(data.GetType());
        public string ForType<T>() where T : IEventData => ForType(typeof(T));
        public string ForType(Type type) => type.Name.ToLower();
    }
}