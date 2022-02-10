using System;
using System.Collections.Generic;
using System.Text;

namespace dht11_project.Sensor
{
    public interface ISensorRep
    {
        bool CheckConnection();
        void AddSensorValue(Model.DataModel newsensvalue);
    }
}
