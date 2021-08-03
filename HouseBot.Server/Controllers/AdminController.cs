using System;
using System.Linq;
using System.Threading.Tasks;
using HouseBot.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseBot.Server.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppData appData;

        public AdminController(AppData appData)
        {
            this.appData = appData;
        }

        [HttpGet("user")]
        public async Task<ActionResult<User[]>> GetAllUsers()
        {
            var users = await appData.Users.ToArrayAsync();
            return Ok(users);
        }

        [HttpPost("user")]
        public async Task<ActionResult<Guid>> RegisterUser(string username)
        {
            username = username.Sanitize();

            var user = await appData.Users.FindAsync(username);
            if(user == null)
            {
                user = new User
                {
                    Name = username,
                };
                appData.Add(user);
            }
            else
            {
                appData.Update(user);
            }

            user.ApiKey = Guid.NewGuid();
            await appData.SaveChangesAsync();

            return Ok(user.ApiKey);
        }

        [HttpDelete("user")]
        public async Task<IActionResult> UnregiserUser(string username)
        {
            username = username.Sanitize();

            var user = await appData.Users.FindAsync(username);

            if(user != null)
            {
                appData.Remove(user);
                await appData.SaveChangesAsync();
            }

            return Ok();
        }
        
        [HttpGet("machine")]
        public async Task<ActionResult<User[]>> GetAllMachines()
        {
            var machines = await appData.ClientMachines.ToArrayAsync();
            return Ok(machines);
        }

        [HttpPost("machine")]
        public async Task<ActionResult<int>> RegisterMachine(string machineName)
        {
            machineName = machineName.Sanitize();

            var machine = await appData.ClientMachines.FindAsync(machineName);
            if(machine == null)
            {
                var machines = await appData.ClientMachines
                    .OrderBy(m => m.PartitionIndex).ToArrayAsync();

                int index;
                for(index = 0; index < machines.Length; index++)
                {
                    if(machines[index].PartitionIndex != index)
                    {
                        break;
                    }
                }

                machine = new ClientMachine
                {
                    Name = machineName,
                    PartitionIndex = index,
                };
                appData.Add(machine);
                await appData.SaveChangesAsync();
            }

            return Ok(machine.PartitionIndex);
        }

        [HttpDelete("machine")]
        public async Task<IActionResult> UnregisterMachine(string machineName)
        {
            machineName = machineName.Sanitize();

            var machine = await appData.ClientMachines.FindAsync(machineName);
            if(machine != null)
            {
                appData.Remove(machine);
                await appData.SaveChangesAsync();
            }

            return Ok();
        }
    }
}