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
        protected Dictionary<int, DateTime> lastUpdateTimes = new Dictionary<int, DateTime>();

        protected List<ControlSensor> controlSensors = new List<ControlSensor>();
        protected List<FanSensor> fanSensors = new List<FanSensor>();
        
        protected virtual TimeSpan throttleMS => TimeSpan.FromMilliseconds(50);
        
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
            // Initialize the controller
            this.HidDevice.Write(new byte[] { 0, this.byteInit, this.byteGet });
        }

        protected void DetectFans()
        {
            // Connect to each port and check if we can get an RPM
            for (int portNumber = 1; portNumber <= this.PortCount; portNumber++)
            {
                int RPM = this.GetFanRPM(portNumber);

                if (RPM > 0)
                {
                    string id = this.GetFanId(portNumber);
                    string name = this.GetFanName(portNumber);

                    Log.WriteToLog($"Creating control sensor {id}: {name} for port {portNumber}");
                    ControlSensor controlSensor = new ControlSensor(id, name, portNumber, this);
                    this.controlSensors.Add(controlSensor);

                    Log.WriteToLog($"Creating fan sensor {id}: {name} for port {portNumber}");
                    FanSensor fanSensor = new FanSensor(id, name, portNumber, this);
                    this.fanSensors.Add(fanSensor);

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

        protected byte[] getPortData(int portNumber)
        {
            DateTime now = DateTime.Now;
            Console.WriteLine("--------------------");
            byte[] portData = new byte[10];

            // If there haven't been updates, we need to update
            bool forceUpdate = !this.lastUpdateTimes.ContainsKey(portNumber);

            if (forceUpdate || now - this.lastUpdateTimes[portNumber] >= this.throttleMS)
            {
                try
                {
                    this.lastUpdateTimes[portNumber] = now;
                    // Send 'get port info' request

                    Console.WriteLine($"Sending the read {this.byteGet}, {this.byteGetSpeed}, {portNumber} request for port {portNumber} to HID device");
                    this.HidDevice.Write(new byte[] { 0, this.byteGet, this.byteGetSpeed, (byte)portNumber });

                    int port = 0;
                    int get = 0;

                    while(get != this.byteGet && port != portNumber)
                    {
                        // Read again
                        int bytesRead = this.HidDevice.Read(portData);
                        port = (int)portData[3];
                        get = (int)portData[1];
                        Console.WriteLine($"Received {bytesRead} bytes from HID Device: \n0 => {portData[0]}\n1 => {portData[1]}\n2 => {portData[2]}\n3 => {portData[3]}\n4 => {portData[4]}\n5 => {portData[5]}\n6 => {portData[6]}\n7 => {portData[7]}");
                    }
                    this.HidDevice.Flush();

                    // Hack to make sure the data is valid check for byteGet and Port number
                    // [0, GET, GETSPEED, PORT, UNKNOWN, SPEED, RPM_L, RPM_H]
                    if (forceUpdate || port == portNumber && get == this.byteGet)
                    {
                        // Store the data
                        this.lastData[portNumber] = portData;
                    }
                } catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to get new data");
                    Console.ResetColor();
                    Console.WriteLine("--------------------");
                    return portData;
                }

                byte[] byteData = this.lastData[portNumber];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"We are returning this data: \n0 => {byteData[0]}\n1 => {byteData[1]}\n2 => {byteData[2]}\n3 => {byteData[3]}\n4 => {byteData[4]}\n5 => {byteData[5]}\n6 => {byteData[6]}\n7 => {byteData[7]}");
                Console.ResetColor();
            } else
            {
                Console.WriteLine($"Returning cached data for port {portNumber}");
            }

            Console.WriteLine("--------------------");
            
            return this.lastData[portNumber];
        }

        public int GetFanRPM(int portNumber)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Getting data for GetFanRPM port {portNumber}");
                Console.ResetColor();

                byte[] portData = this.getPortData(portNumber);
                int RPM = (portData[7] << 8) + portData[6];

                RPM = (RPM > 0) ? RPM : 0;

                // Added this for throttling of HID device
                return RPM;
            } catch
            {
                Log.WriteToLog("RPM timeout");
                return 0;
            }
        }

        public int GetFanPower(int portNumber)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Getting data for GetFanPower port {portNumber}");
            Console.ResetColor();

            try
            {
                byte[] portData = this.getPortData(portNumber);

                int power = portData[5];
                power = (power > 0) ? power : 0;

                return power;
            }
            catch
            {
                return 0;
            }

        }

        public void SetFanPower(int portNumber, float value)
        {
            Log.WriteToLog($"Setting fan {portNumber} to {value}%");
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
