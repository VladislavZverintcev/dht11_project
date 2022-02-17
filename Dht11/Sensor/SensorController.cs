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
                Debug.WriteLine($"Dht11_project: Failed database connection, trying again. {DateTime.UtcNow.ToString("G")}");
                Thread.Sleep(5000);
            }
            using (Dht11 dht = new Dht11(_config.Dht11_pin, PinNumberingScheme.Logical, gpio))
            {
                while(true)
                {
                    Model.DataModel newsensvalue = null;
                    int realdelay = _config.SensorDelay;
                    while (newsensvalue == null)
                    {
                        Temperature temperature;
                        dht.TryReadTemperature(out temperature);
                        RelativeHumidity rHumidity;
                        dht.TryReadHumidity(out rHumidity);
                        if (dht.IsLastReadSuccessful)
                        {
                            newsensvalue = new Model.DataModel
                            {
                                RegistredDateTime = DateTime.UtcNow.Ticks,
                                Temperature = Convert.ToDecimal(temperature.DegreesCelsius),
                                Humidity = Convert.ToDecimal(rHumidity.Percent)
                            };
                            try
                            {
                                rep.AddSensorValue(newsensvalue);
                            }
                            catch
                            {
                                Debug.WriteLine($"Dht11_project: Failed to add SensorValue. Probably database connection is lost. {DateTime.UtcNow.ToString("G")}");
                            }
                        }
                        if((realdelay - 2000) >= 0)
                        {
                            realdelay = realdelay - 2000;
                        }
                        else { realdelay = 0; }
                        Thread.Sleep(2000);
                    }
                    Thread.Sleep(realdelay);
                }
                
            }
        }
    }
}
