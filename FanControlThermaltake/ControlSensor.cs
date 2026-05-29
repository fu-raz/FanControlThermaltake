using FanControl.Plugins;

namespace FanControl.Thermaltake
{
    public class ControlSensor : IPluginControlSensor2
    {
        public string Id { get; }
        public string Name { get; }
        public float? Value { get; set; }
        public string PairedFanSensorId => this.Id;

        protected int portNumber;
        protected TTFanControllerInterface ttFanController;

        public ControlSensor(string id, string name, int portNumber, TTFanControllerInterface ttFanController)
        {
            this.Id = id;
            this.Name = name;
            this.portNumber = portNumber;
            this.ttFanController = ttFanController;
            this.Update();
        }

        public void Reset()
        {
            this.ttFanController.SetFanPower(this.portNumber, 25);
        }

        public void Set(float val)
        {
            this.ttFanController.SetFanPower(this.portNumber, val);
            this.Value = val;
        }

        public void Update()
        {
            this.Value = this.ttFanController.GetFanPower(this.portNumber);
        }
    }
}
