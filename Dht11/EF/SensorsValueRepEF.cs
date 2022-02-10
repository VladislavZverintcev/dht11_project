using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using dht11_project.Sensor;

namespace dht11_project.EF
{
    class SensorsValueRepEF : ISensorRep
    {
        ConfigData _config;
        public SensorsValueRepEF(ConfigData config)
        {
            _config = config;
        }
        public bool CheckConnection()
        {
            using (var context = new DBcontextEF(_config))
            {
                if(context.Database.CanConnect())
                { return true; }
                else { return false; }
            }
        }
        public void AddSensorValue(Model.DataModel newsensvalue)
        {
            try
            {
                using (var context = new DBcontextEF(_config))
                {
                    context.SensValues.Add(newsensvalue);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
