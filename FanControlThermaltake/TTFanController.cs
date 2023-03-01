using System;
using System.Collections.Generic;

namespace FanControl.ThermaltakeRiingPlus
{
    public class TTFanController : TTFanControllerInterface
    {
        public HidSharp.HidStream HidDevice;
        public int Index;
        public virtual string Name => "Default Controller";
        public virtual int PortCount => 5;

        public virtual int ProductIdStart => 0;
        public virtual int ProductIdEnd => 5;

        public virtual byte byteGet => 0x33;
        public virtual byte byteGetSpeed => 0x51;
        public virtual byte byteSet => 0x32;
        public virtual byte byteSetSpeed => 0x01;
        public virtual byte byteInit => 0xfe;

        protected List<ControlSensor> controlSensors = new List<ControlSensor>();
        protected List<FanSensor> fanSensors = new List<FanSensor>();

        public TTFanController()
        {
           
        }

        public void init(HidSharp.HidStream hidDevice, int index)
        {
            this.HidDevice = hidDevice;
            this.Index = index;

            this.InitController();
            this.DetectFans();
        }

        protected void InitController()
        {
            // Initialize the controller
            this.HidDevice.Write(new byte[] { 0, this.byteInit, this.byteGet });
        }

        public virtual string GetName() => Name;
        protected void DetectFans()
        {
            // Connect to each port and check if we can get an RPM
            for (int portNumber = 1; portNumber <= this.PortCount; portNumber++)
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
            return $"TT.{this.Index}.{portNumber}";
        }

        protected string GetFanName(int portNumber)
        {
            return $"Fan {portNumber} on {this.GetName()} Controller {this.Index}";
        }

        public int GetFanRPM(int portNumber)
        {
            try
            {
                // Send 'get port info' request
                this.HidDevice.Write(new byte[] { 0, this.byteGet, this.byteGetSpeed, (byte)portNumber });
                // Read the output
                byte[] portData = new byte[10];

                this.HidDevice.Read(portData);
                // If we have an RPM of more than 255, we assume it's a fan
                int RPM = (portData[7] << 8) + portData[6];

                return (RPM > 0) ? RPM : 0;
            } catch(Exception ex)
            {
                Log.WriteToLog(ex.ToString());
                return 0;
            }
        }

        public int GetFanPower(int portNumber)
        {
            // Send 'get port info' request
            this.HidDevice.Write(new byte[] { 0x00, this.byteGet, this.byteGetSpeed, (byte)portNumber });
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
            this.HidDevice.Write(new byte[] { 0, this.byteSet, this.byteGetSpeed, (byte)portNumber, this.byteSetSpeed, (byte)percentage });
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
