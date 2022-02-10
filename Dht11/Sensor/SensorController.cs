using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Threading;
using Iot.Device.DHTxx;
using IoT.Device;
using UnitsNet;
using dht11_project.EF;
using System.Diagnostics;

namespace dht11_project.Sensor
{
    public class SensorController
    {
        LibGpiodDriver rpd;
        GpioController gpio;
        ConfigData _config;

        public SensorController(ConfigData config)
        {
            _config = config;
            rpd = new LibGpiodDriver();
            gpio = new GpioController(PinNumberingScheme.Logical, rpd);
            
        }
        public void Start()
        {
            
            ISensorRep rep = new SensorsValueRepEF(_config);
            while (!rep.CheckConnection())
            {
                Debug.WriteLine($"Dht11 demon: Failed database connection, trying again. {DateTime.UtcNow.ToString("G")}");
                Thread.Sleep(5000);
            }
            using (Dht11 dht = new Dht11(_config.Dht11_pin, PinNumberingScheme.Logical, gpio))
            {
                while(true)
                {
                    Temperature temperature;
                    dht.TryReadTemperature(out temperature);
                    RelativeHumidity rHumidity;
                    dht.TryReadHumidity(out rHumidity);
                    if (dht.IsLastReadSuccessful)
                    {
                        Model.DataModel newsensvalue = new Model.DataModel
                        {
                            RegistredDateTimeG = DateTime.UtcNow.ToString("G"),
                            Temperature = Convert.ToDecimal(temperature.DegreesCelsius),
                            Humidity = Convert.ToDecimal(rHumidity.Percent)
                        };
                        try
                        {
                            rep.AddSensorValue(newsensvalue);
                        }
                        catch
                        {
                            Debug.WriteLine($"Dht11 demon: Failed to add SensorValue. Probably database connection is lost. {DateTime.UtcNow.ToString("G")}");
                        }
                    }
                    Thread.Sleep(_config.SensorDelay);
                }
                
            }
        }
    }
}
