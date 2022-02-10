using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;


namespace dht11_project.EF
{
    class DBContext : DbContext
    {
        public DBContext()
        {
            Database.EnsureCreated();
        }
        public DbSet<Model.DataModel> SensValues { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=localhost;Database=Sensor1_dht11;Uid=SensorAdmin;Pwd=sensor;");
        }
    }
}