using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Threading;
using Iot.Device.DHTxx;
using IoT.Device;
using UnitsNet;
using dht11_project.EF;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace dht11_project.Sensor
{
    public class SensorController
    {
        LibGpiodDriver rpd;
        GpioController gpio;
        ConfigData _config;
        double tolerance = 1.25;
        int decimalRound = 1;

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
                    int realdelay = _config.SensorDelay;
                    var bashOfValues = new List<Model.DataModel>();
                    while (realdelay > 0)
                    {
                        Model.DataModel newsensvalue = null;
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
                                bashOfValues.Add(newsensvalue);
                            }
                            if ((realdelay - 2000) >= 0)
                            {
                                realdelay = realdelay - 2000;
                            }
                            else { realdelay = 0; }
                            Thread.Sleep(2000);
                        }
                    }
                    var fixedvalue = GetNormalizeValues(bashOfValues);
                    if(fixedvalue != null)
                    try
                    {
                        rep.AddSensorValue(fixedvalue);
                    }
                    catch
                    {
                        Debug.WriteLine($"Dht11_project: Failed to add SensorValue. Probably database connection is lost. {DateTime.UtcNow.ToString("G")}");
                    }
                }
            }
        }
        Model.DataModel GetNormalizeValues(List<Model.DataModel> listOfValues)
        {
            var temp = new List<decimal>();
            var humi = new List<decimal>();

            var resulttemp = new List<decimal>();
            var resulthumi = new List<decimal>();

            var result = new Model.DataModel();

            if (listOfValues.Count == 1)
            {
                return listOfValues[0];
            }
            if (listOfValues.Count == 0)
            {
                return null;
            }
            if (listOfValues.Count == 2)
            {
                result.Temperature = (listOfValues[0].Temperature + listOfValues[1].Temperature) / 2;
                result.Humidity = (listOfValues[0].Humidity + listOfValues[1].Humidity) / 2;
                result.RegistredDateTime = DateTime.UtcNow.Ticks;
                result.Temperature = decimal.Round(result.Temperature, decimalRound);
                result.Humidity = decimal.Round(result.Humidity, decimalRound);
                return result;
            }
            foreach (var value in listOfValues)
            {
                temp.Add(value.Temperature);
                humi.Add(value.Humidity);
            }

            for (int i = 0; i < listOfValues.Count; i++)
            {
                var middlevalueoftemp = ((temp.Aggregate((x, y) => x + y)) - temp[i]) / temp.Count - 1;
                var middlevalueofhumi = ((humi.Aggregate((x, y) => x + y)) - humi[i]) / humi.Count - 1;

                if(temp[i] >= middlevalueoftemp)
                {
                    if (temp[i] <= middlevalueoftemp * Convert.ToDecimal(tolerance))
                    {
                        resulttemp.Add(temp[i]);
                    }
                }
                else
                {
                    if (temp[i] >= middlevalueoftemp / Convert.ToDecimal(tolerance))
                    {
                        resulttemp.Add(temp[i]);
                    }
                }
                if (humi[i] >= middlevalueofhumi)
                {
                    if (humi[i] <= middlevalueofhumi * Convert.ToDecimal(tolerance))
                    {
                        resulthumi.Add(humi[i]);
                    }
                }
                else
                {
                    if (humi[i] >= middlevalueofhumi / Convert.ToDecimal(tolerance))
                    {
                        resulthumi.Add(humi[i]);
                    }
                }
            }
            result.Temperature = resulttemp.Aggregate((x, y) => x + y) / resulttemp.Count;
            result.Humidity = resulthumi.Aggregate((x, y) => x + y) / resulthumi.Count;
            result.RegistredDateTime = DateTime.UtcNow.Ticks;
            result.Temperature = decimal.Round(result.Temperature, decimalRound);
            result.Humidity = decimal.Round(result.Humidity, decimalRound);
            return result;
        }
    }
}
