using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace dht11_project.EF
{
    class SensorsValueRep
    {
        public static bool CheckConnection()
        {
            using (var context = new DBContext())
            {
                if(context.Database.CanConnect())
                { return true; }
                else { return false; }
            }
        }
        public static void AddSensorValue(Model.DataModel newsensvalue)
        {
            try
            {
                using (var context = new DBContext())
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
