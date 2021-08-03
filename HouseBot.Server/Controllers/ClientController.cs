using System.Threading.Tasks;
using HouseBot.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseBot.Server.Controllers
{
    public class ClientController : Controller
    {
        private readonly GetPartitionIndex getPartitionIndex;

        public ClientController(GetPartitionIndex getPartitionIndex)
        {
            this.getPartitionIndex = getPartitionIndex;
        }

        [HttpGet]
        public async Task<ActionResult<int>> GetPartitionIndex(string machineName)
        {
            var index = await getPartitionIndex.ForMachineNameAsync(machineName);
            return Ok(index);
        }
    }
}