using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;


namespace dht11_project.EF
{
    class DBcontextEF : DbContext
    {
        string connectionString;
        public DBcontextEF(ConfigData config)
        {
            connectionString = config.DBConnectionString;
            Database.EnsureCreated();
        }
        public DbSet<Model.DataModel> SensValues { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString);
        }
    }
}