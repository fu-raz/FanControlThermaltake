using FanControl.Plugins;
using System.Collections.Generic;

namespace FanControl.ThermaltakeRiingPlus
{
    public class Plugin : IPlugin2
    {
        private DevicesController DevicesController = new DevicesController();
        public string Name => "Thermaltake";

        public void Close()
        {
            this.DevicesController.Disconnect();
        }

        public void Initialize()
        {
            Log.WriteToLog("Initializing Plugin");
            this.DevicesController.Connect();
        }

        public void Load(IPluginSensorsContainer container)
        {
            Log.WriteToLog("Loading Sensors");

            List<ControlSensor> controlSensors = new List<ControlSensor>();
            List<FanSensor> fanSensors = new List<FanSensor>();

            List<TTFanController> fanControllers = this.DevicesController.GetFanControllers();
            
            foreach (TTFanController fanController in fanControllers)
            {
                List<ControlSensor> cs = fanController.GetControlSensors();
                foreach (ControlSensor sensor in cs)
                {
                    Log.WriteToLog($"We found {sensor.Name} at {sensor.Value}% power");
                    controlSensors.Add(sensor);
                }

                List<FanSensor> fs = fanController.GetFanSensors();
                foreach (FanSensor sensor in fs)
                {
                    Log.WriteToLog($"We found {sensor.Name} at {sensor.Value} RPM");
                    fanSensors.Add(sensor);
                }
            }

            Log.WriteToLog($"We found {controlSensors.Count} Control sensors and {fanSensors.Count} Fan sensors");

            if (controlSensors.Count > 0) container.ControlSensors.AddRange(controlSensors);
            if (fanSensors.Count > 0) container.FanSensors.AddRange(fanSensors);
        }

        public void Update()
        {

        }
    }
}
