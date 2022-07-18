using FanControl.Plugins;

namespace FanControl.ThermaltakeRiingPlus
{
    public class ControlSensor : IPluginControlSensor
    {
        public string Id { get; }
        public string Name { get; }
        public float? Value { get; set; }

        protected int portNumber;
        protected TTFanController ttFanController;

        public ControlSensor(string id, string name, int portNumber, TTFanController ttFanController)
        {
            this.Id = id;
            this.Name = name;
            this.portNumber = portNumber;
            this.ttFanController = ttFanController;
            this.Update();
        }

        public void Reset()
        {
            this.ttFanController.SetFanPower(this.portNumber, 50);
        }

        public void Set(float val)
        {
            this.ttFanController.SetFanPower(this.portNumber, val);
        }

        public void Update()
        {
            this.Value = this.ttFanController.GetFanPower(this.portNumber);
        }
    }
}
