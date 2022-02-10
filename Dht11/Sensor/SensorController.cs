using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Threading;
using Iot.Device.DHTxx;
using IoT.Device;
using UnitsNet;
using dht11_project.EF;

namespace dht11_project.Sensor
{
    public class SensorController
    {
        LibGpiodDriver rpd;
        GpioController gpio;
        int _interval;
        bool enable = false;

        public SensorController(int interval)
        {
            _interval = interval;
            rpd = new LibGpiodDriver();
            gpio = new GpioController(PinNumberingScheme.Logical, rpd);
            
        }
        public void Start()
        {
            enable = true;
            using (Dht11 dht = new Dht11(26, PinNumberingScheme.Logical, gpio))
            {
                Console.WriteLine($"------------Подключение к базе------------");
                while (!SensorsValueRep.CheckConnection())
                {
                    Console.WriteLine($"------------Попытка подключиться к базе------------");
                    Thread.Sleep(2000);
                }
                Console.WriteLine($"Подключение успешно!");
                while (enable)
                {
                    Temperature temperature;
                    //Temperature temperature = dht.Temperature;
                    dht.TryReadTemperature(out temperature);
                    RelativeHumidity rHumidity;
                    dht.TryReadHumidity(out rHumidity);

                    if (dht.IsLastReadSuccessful)
                    {
                        Console.WriteLine($"Новое показание!");
                        Model.DataModel newsensvalue = new Model.DataModel
                        {
                            RegistredDateTimeG = DateTime.UtcNow.ToString("G"),
                            Temperature = Convert.ToDecimal(temperature.DegreesCelsius),
                            Humidity = Convert.ToDecimal(rHumidity.Percent)
                        };
                        if(SensorsValueRep.CheckConnection())
                        {
                            SensorsValueRep.AddSensorValue(newsensvalue);
                        }
                    }
                    Thread.Sleep(_interval);
                }
            }
        }
        public void Stop()
        {
            enable = false;
        }
    }
}
