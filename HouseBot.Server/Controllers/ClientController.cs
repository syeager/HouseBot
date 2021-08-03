using System.Threading.Tasks;
using HouseBot.Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace HouseBot.Server.Controllers
{
    [ApiController]
    [Route("client")]
    public class ClientController : ControllerBase
    {
        private readonly AppData appData;

        public ClientController(AppData appData)
        {
            this.appData = appData;
        }

        [HttpGet]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<ActionResult<int>> GetPartitionIndex(string machineName)
        {
            machineName = machineName.Sanitize();

            var machine = await appData.ClientMachines.FindAsync(machineName);
            if(machine == null)
            {
                return NotFound(machineName);
            }

            return Ok(machine.PartitionIndex);
        }
    }
}