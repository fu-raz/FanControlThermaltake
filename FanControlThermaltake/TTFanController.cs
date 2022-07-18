using System;
using System.Collections.Generic;

namespace FanControl.ThermaltakeRiingPlus
{
    public class TTFanController
    {
        public HidSharp.HidStream HidDevice;
        public int Index;
        public string Name => "Thermaltake Riing";
        public int PortCount = 5;

        protected List<ControlSensor> controlSensors = new List<ControlSensor>();
        protected List<FanSensor> fanSensors = new List<FanSensor>();

        public TTFanController(HidSharp.HidStream hidDevice, int index)
        {
            this.HidDevice = hidDevice;
            this.Index = index;

            // Initialize
            this.DetectFans();
        }

        protected void DetectFans()
        {
            // Connect to each port and check if we can get an RPM
            for (int portNumber = 1; portNumber < this.PortCount; portNumber++)
            {
                Log.WriteToLog($"Trying to detect Fan on port {portNumber}");
                int RPM = this.GetFanRPM(portNumber);

                if (RPM > 0)
                {
                    Log.WriteToLog($"Found RPM of {RPM}");
                    ControlSensor controlSensor = new ControlSensor(this.GetFanId(portNumber), this.GetFanName(portNumber), portNumber, this);
                    this.controlSensors.Add(controlSensor);

                    FanSensor fanSensor = new FanSensor(this.GetFanId(portNumber), this.GetFanName(portNumber), portNumber, this);
                    this.fanSensors.Add(fanSensor);
                } else
                {
                    Log.WriteToLog("NO RPM could be detected");
                }
            }
        }

        protected string GetFanId(int portNumber)
        {
            return $"TTRiing.{this.Index}.{portNumber}";
        }

        protected string GetFanName(int portNumber)
        {
            return $"{this.Name} Fan {portNumber} on Controller {this.Index}";
        }

        public int GetFanRPM(int portNumber)
        {
            // Send 'get port info' request
            this.HidDevice.Write(new byte[] { 0x00, 0x33, 0x51, (byte)portNumber });
            // Read the output
            byte[] portData = new byte[10];
            this.HidDevice.Read(portData);
            // If we have an RPM of more than 255, we assume it's a fan
            int RPM = (portData[7] << 8) + portData[6];

            return (RPM > 0) ? RPM : 0;
        }

        public int GetFanPower(int portNumber)
        {
            // Send 'get port info' request
            this.HidDevice.Write(new byte[] { 0x00, 0x33, 0x51, (byte)portNumber });
            // Read the output
            byte[] portData = new byte[10];
            this.HidDevice.Read(portData);
            // If we have an RPM of more than 255, we assume it's a fan
            int power = portData[5];

            return (power > 0) ? power : 0;
        }

        public void SetFanPower(int portNumber, float value)
        {
            int percentage = (int)Math.Round(value);
            this.HidDevice.Write(new byte[] { 0, 0x32, 0x51, (byte)portNumber, 0x01, (byte)percentage });
        }

        public List<ControlSensor> GetControlSensors()
        {
            return this.controlSensors;
        }
        public List<FanSensor> GetFanSensors()
        {
            return this.fanSensors;
        }
    }
}
