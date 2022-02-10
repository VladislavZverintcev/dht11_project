using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Threading;
using Iot.Device.DHTxx;
using IoT.Device;
using UnitsNet;
using dht11_project.Sensor;
using System.Diagnostics;

namespace dht11_project
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Dht11 demon! For exit press Ctrl + C");
            ConfigWorker cw = new ConfigWorker();
            ConfigData config = cw.GetConfig();
            if(config.DBConnectionString == "SomeConnectionString")
            {
                Console.WriteLine("Please configure config.xml");
                Debug.WriteLine($"Dht11 demon: Please configure config.xml {DateTime.UtcNow.ToString("G")}");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            SensorController sCont = new SensorController(config);
            sCont.Start();
        }
        
    }
}
