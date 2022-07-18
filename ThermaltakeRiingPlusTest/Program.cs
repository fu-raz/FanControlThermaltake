
using FanControl.ThermaltakeRiingPlus;
using System;
using System.Collections.Generic;

namespace ThermaltakeRiingPlusTest
{
    class Program
    {
        protected List<TTFanController> fanControllers = new List<TTFanController>();
        static void Main(string[] args)
        {
            DevicesController d = new DevicesController();
            d.Connect();


            Log.WriteToLog("Trying to get all fan controllers");
            List<TTFanController> l = d.GetFanControllers();

            Log.WriteToLog($"We got {l.Count} controllers");

            show(l);

            setSpeed(l, 100);

            show(l);

            Boolean exitRequested = false;

            while (!exitRequested)
            {
            }
        }

        protected static void show(List<TTFanController> fanControllers)
        {
            foreach (TTFanController fanController in fanControllers)
            {
                List<ControlSensor> cs = fanController.GetControlSensors();
                foreach (ControlSensor sensor in cs)
                {
                    Log.WriteToLog($"We found {sensor.Name} at {sensor.Value}% power");
                }

                List<FanSensor> fs = fanController.GetFanSensors();
                Log.WriteToLog($"We found {fs.Count} sensors");
                foreach (FanSensor sensor in fs)
                {
                    Log.WriteToLog($"We found {sensor.Name} at {sensor.Value} RPM");
                }
            }
        }

        protected static void setSpeed(List<TTFanController> fanControllers, int speed)
        {
            foreach (TTFanController fanController in fanControllers)
            {
                List<ControlSensor> cs = fanController.GetControlSensors();
                foreach (ControlSensor sensor in cs)
                {
                    sensor.Set(speed);
                }
            }
        }

    }
}