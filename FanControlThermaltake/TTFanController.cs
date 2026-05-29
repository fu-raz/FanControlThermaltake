using System;
using System.Collections.Generic;

namespace FanControl.Thermaltake
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

        protected int productId = 0;

        protected Dictionary<int, byte[]> lastData = new Dictionary<int, byte[]>();
        protected List<int> activePorts = new List<int>();

        protected List<ControlSensor> controlSensors = new List<ControlSensor>();
        protected List<FanSensor> fanSensors = new List<FanSensor>();

        public TTFanController()
        {
        }

        public void init(HidSharp.HidStream hidDevice, int index, int productId)
        {
            this.HidDevice = hidDevice;
            this.Index = index;
            this.productId = productId;

            this.InitController();
            this.DetectFans();
        }

        protected void InitController()
        {
            this.lastData.Clear();
            this.activePorts.Clear();
            this.HidDevice.Write(new byte[] { 0, this.byteInit, this.byteGet });
            // Read and discard the init response so it doesn't corrupt subsequent reads.
            // response[3] holds the status byte (0xfc=ok); response[1] echoes byteInit (0xfe) which is not a failure code.
            byte[] response = new byte[64];
            try { this.HidDevice.Read(response); } catch { }
            Log.WriteToLog($"Init response: cmd=0x{response[1]:X2} status=0x{response[3]:X2}");
        }

        protected void DetectFans()
        {
            for (int portNumber = 1; portNumber <= this.PortCount; portNumber++)
            {
                this.getPortData(portNumber);
                int RPM = this.GetFanRPM(portNumber);

                if (RPM > 0)
                {
                    string id = this.GetFanId(portNumber);
                    string name = this.GetFanName(portNumber);

                    Log.WriteToLog($"Creating control sensor {id}: {name} for port {portNumber}");
                    this.controlSensors.Add(new ControlSensor(id, name, portNumber, this));

                    Log.WriteToLog($"Creating fan sensor {id}: {name} for port {portNumber}");
                    this.fanSensors.Add(new FanSensor(id, name, portNumber, this));

                    this.activePorts.Add(portNumber);
                    Log.WriteToLog("---");
                }
            }
        }

        protected string GetFanId(int portNumber)
        {
            return $"{this.productId}/{portNumber}";
        }

        protected string GetFanName(int portNumber)
        {
            string controllerSuffix = this.Index > 0 ? " " + (this.Index + 1).ToString() : "";
            return $"Fan {portNumber} on TT {this.Name} Controller{controllerSuffix}";
        }

        // Polls the HID device for a single port and caches the result in lastData.
        // The controller continuously emits autonomous status reports (cmd=0x32) between responses,
        // so we use a time-based window instead of a fixed retry count to drain through them.
        protected void getPortData(int portNumber)
        {
            byte[] portData = new byte[64];
            try
            {
                Log.WriteToLog($"Polling port {portNumber}");
                this.HidDevice.Write(new byte[] { 0, this.byteGet, this.byteGetSpeed, (byte)portNumber });

                int port = 0;
                int get = 0;
                var deadline = DateTime.Now.AddMilliseconds(500);

                while ((get != this.byteGet || port != portNumber) && DateTime.Now < deadline)
                {
                    int bytesRead = this.HidDevice.Read(portData);
                    port = (int)portData[3];
                    get = (int)portData[1];
                    Log.WriteToLog($"Read {bytesRead} bytes: cmd={get:X2} port={port}");
                }
                this.HidDevice.Flush();

                if (port == portNumber && get == this.byteGet)
                {
                    this.lastData[portNumber] = portData;
                }
                else
                {
                    Log.WriteToLog($"Port {portNumber}: no valid response within timeout");
                }
            }
            catch (Exception ex)
            {
                Log.WriteToLog($"Port {portNumber} poll failed: {ex.Message}");
            }
        }

        public void PollAllPorts()
        {
            foreach (int portNumber in this.activePorts)
                this.getPortData(portNumber);
        }

        public int GetFanRPM(int portNumber)
        {
            try
            {
                if (this.lastData.TryGetValue(portNumber, out byte[] portData))
                {
                    int RPM = (portData[7] << 8) + portData[6];
                    return RPM > 0 ? RPM : 0;
                }
                return 0;
            }
            catch
            {
                Log.WriteToLog($"GetFanRPM failed for port {portNumber}");
                return 0;
            }
        }

        public int GetFanPower(int portNumber)
        {
            try
            {
                if (this.lastData.TryGetValue(portNumber, out byte[] portData))
                {
                    int power = portData[5];
                    return power > 0 ? power : 0;
                }
                return 0;
            }
            catch
            {
                Log.WriteToLog($"GetFanPower failed for port {portNumber}");
                return 0;
            }
        }

        public void SetFanPower(int portNumber, float value)
        {
            int percentage = (int)Math.Round(value);
            Log.WriteToLog($"Setting fan {portNumber} to {percentage}%");
            this.HidDevice.Write(new byte[] { 0, this.byteSet, this.byteGetSpeed, (byte)portNumber, this.byteSetSpeed, (byte)percentage });
            // Read and discard the set response to keep the read buffer clean.
            // response[1] echoes byteSet (0x32); actual status is at response[3] (0xfc=ok, 0xfe=fail).
            byte[] response = new byte[64];
            try { this.HidDevice.Read(response); } catch { }
            Log.WriteToLog($"Set response: cmd=0x{response[1]:X2} status=0x{response[3]:X2} (0xfc=ok, 0xfe=fail)");
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
