using System;

namespace HouseBot.Server.Authorization
{
    public class ApiKeyGenerator
    {
        public string New()
        {
            return Guid.NewGuid().ToString();
        }
    }
}