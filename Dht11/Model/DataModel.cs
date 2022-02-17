using System;
using System.Collections.Generic;
using System.Text;

namespace dht11_project.Model
{
    public class DataModel
    {
        public int Id { get; set; }
        public long RegistredDateTime { get; set; }
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
    }
}
