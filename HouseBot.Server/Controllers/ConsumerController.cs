using System;
using System.Threading.Tasks;
using HouseBot.Client.Events;
using HouseBot.Events.Core;
using HouseBot.Server.Authorization;
using HouseBot.Server.Core;
using Microsoft.AspNetCore.Mvc;

namespace HouseBot.Server.Controllers
{
    public abstract class ConsumerController<T> : Controller where T: IEventData
    {
        private readonly EventConsumer<T> eventConsumer;
        private readonly ApiKeyStore apiKeyStore;

        protected ConsumerController(EventConsumer<T> eventConsumer, ApiKeyStore apiKeyStore)
        {
            this.eventConsumer = eventConsumer;
            this.apiKeyStore = apiKeyStore;
        }

        [HttpPost]
        public async Task<IActionResult> Fire(EventRequest<T> request)
        {
            var user = apiKeyStore.GetUser(request.ApiKey);
            var @event = new Event<T>(Guid.NewGuid(), DateTime.UtcNow, user, request.Data);
            await eventConsumer.ConsumeAsync(@event);
            return Ok();
        }
    }
}