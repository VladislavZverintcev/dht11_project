using System;
using System.Collections.Generic;
using System.Text;

namespace dht11_project
{
    [Serializable]
    public class ConfigData
    {
        public int Dht11_pin { get; set; }
        public int SensorDelay { get; set; }
        public string DBConnectionString { get; set; }
    }
}
