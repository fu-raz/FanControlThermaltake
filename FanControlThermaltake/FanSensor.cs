using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FanControl.Plugins;

namespace FanControl.ThermaltakeRiingPlus
{
    public class FanSensor : IPluginSensor
    {
        public string Id { get; }
        public string Name { get; }
        public float? Value { get; set; }

        protected int portNumber;
        protected TTFanControllerInterface ttFanController;

        public FanSensor(string id, string name, int portNumber, TTFanControllerInterface ttFanController)
        {
            this.Id = id;
            this.Name = name;
            this.portNumber = portNumber;
            this.ttFanController = ttFanController;
            this.Update();
        }

        public void Update()
        {
            this.Value = this.ttFanController.GetFanRPM(this.portNumber);
        }
    }
}
