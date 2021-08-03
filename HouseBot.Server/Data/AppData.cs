using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HouseBot.Server.Data
{
    public class ClientMachine
    {
        [Key]
        public string Name { get; set; }
        public int PartitionIndex { get; set; }
    }

    public class User
    {
        [Key]
        public string Name { get; set; }
        public Guid ApiKey { get; set; }
    }

    public class AppData : DbContext
    {
        public DbSet<ClientMachine> ClientMachines { get; set; }
        public DbSet<User> Users { get; set; }

        public AppData(DbContextOptions<AppData> options)
            : base(options)
        {
        }
    }
}