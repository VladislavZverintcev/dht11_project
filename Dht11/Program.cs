using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Threading;
using Iot.Device.DHTxx;
using IoT.Device;
using UnitsNet;
using dht11_project.Sensor;

namespace dht11_project
{

    class Program
    {
        static void Main(string[] args)
        {
            SensorController sCont = new SensorController(5000);
            sCont.Start();
        }
    }
}
