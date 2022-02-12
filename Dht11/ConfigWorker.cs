using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace dht11_project
{
    public class ConfigWorker
    {
        string configPath = @$"{AppDomain.CurrentDomain.BaseDirectory}config.xml";
        ConfigData defaultConfig = new ConfigData
        {
            DBConnectionString = "SomeConnectionString",
            Dht11_pin = 26,
            SensorDelay = 5000
        };
        public ConfigData GetConfig()
        {
            if (File.Exists(configPath))
            {
                try
                {
                    TextReader tr = new StreamReader(configPath);
                    return Read(tr);
                }
                catch
                {
                    Debug.WriteLine($"Dht11_project: cofig.xml reading failed! {DateTime.UtcNow.ToString("G")}");
                    return defaultConfig;
                }
            }
            else
            {
                Debug.WriteLine($"Dht11_project: cofig.xml not exist, creating... {DateTime.UtcNow.ToString("G")}");
                try
                {
                    TextWriter tw = new StreamWriter(configPath);
                    Write(tw, defaultConfig);
                    tw.Close();
                    Debug.WriteLine($"Dht11_project: Please configure config.xml {DateTime.UtcNow.ToString("G")}");
                    return defaultConfig;
                }
                catch
                {
                    Debug.WriteLine($"Dht11_project: cofig.xml creating failed! {DateTime.UtcNow.ToString("G")}");
                    return defaultConfig;
                }
            }
        }
        void Write(TextWriter writer, ConfigData config)
        {
            XmlSerializer x = new XmlSerializer(typeof(ConfigData));
            x.Serialize(writer, config);
        }
        ConfigData Read(TextReader reader)
        {
            XmlSerializer x = new XmlSerializer(typeof(ConfigData));
            return (ConfigData)x.Deserialize(reader);
        }
    }
}
