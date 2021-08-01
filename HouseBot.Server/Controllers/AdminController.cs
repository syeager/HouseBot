using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HouseBot.Server.Controllers
{
    public class AdminController : Controller
    {
        [HttpPost]
        public Task<ActionResult<string>> CreateApiKey(string user)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public Task<IActionResult> DeleteApiKey(string apiKey)
        {
            throw new NotImplementedException();
        }
    }
}