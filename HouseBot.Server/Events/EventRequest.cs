﻿using System;
using HouseBot.Data.Core;

namespace HouseBot.Server.Events
{
    public sealed class EventRequest<T> where T : IEventData
    {
        public Guid ApiKey { get; set; }
        public string Target { get; set; }
        public T Data { get; set; }
    }
}