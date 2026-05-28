using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace FanControl.Thermaltake
{
    public class DevicesController
    {
        protected int VendorId = 0x264a;

        protected int MaxConnectedDevices = 5;
        protected bool isConnected = false;
        protected List<TTFanControllerInterface> Devices = new List<TTFanControllerInterface>();
        protected List<HidSharp.HidStream> openStreams = new List<HidSharp.HidStream>();

        public void Connect()
        {
            if (!this.isConnected)
            {
                IEnumerable<HidSharp.HidDevice> deviceList = HidSharp.DeviceList.Local.GetHidDevices();
                foreach (HidSharp.HidDevice hidDevice in deviceList)
                {
                    if (hidDevice.VendorID == this.VendorId)
                    {
                        Log.WriteToLog($"Found Thermaltake device with Vendor ID {hidDevice.VendorID} and Product ID {hidDevice.ProductID}");
                        TTFanControllerInterface fanControllerMatch = this.findController(hidDevice.ProductID);
                        if (null != fanControllerMatch)
                        {
                            Log.WriteToLog($"We found a TT device: {fanControllerMatch.Name}");

                            if (hidDevice.TryOpen(out HidSharp.HidStream hidStream))
                            {
                                hidStream.ReadTimeout = 1000;
                                hidStream.WriteTimeout = 1000;

                                int controllerIndex = this.Devices.Count;
                                fanControllerMatch.init(hidStream, controllerIndex, hidDevice.ProductID);

                                Log.WriteToLog("Adding HID Device to Devices");
                                this.Devices.Add(fanControllerMatch);
                                this.openStreams.Add(hidStream);
                                Log.WriteToLog($"We have {this.Devices.Count} devices");
                            }
                        }
                        else
                        {
                            Log.WriteToLog("We found a TT device, but it isn't supported yet. Please post an issue here (https://github.com/fu-raz/FanControlThermaltake/issues/) with this log");
                        }
                    }
                }

                this.isConnected = true;
            }
        }

        private TTFanControllerInterface findController(int hidDeviceProductId)
        {
            string targetNamespace = "FanControl.Thermaltake.FanControllers";
            var matchingTypes = Assembly.GetExecutingAssembly()
                                    .GetTypes()
                                    .Where(t => t.Namespace == targetNamespace);

            foreach (var match in matchingTypes)
            {
                Log.WriteToLog($"Found a FanController class: {match.Name}");
                TTFanControllerInterface ttFanController = System.Activator.CreateInstance(match) as TTFanControllerInterface;
                Log.WriteToLog($"Testing supported controller {ttFanController.Name}");
                if (hidDeviceProductId >= ttFanController.ProductIdStart && hidDeviceProductId <= ttFanController.ProductIdEnd)
                {
                    return ttFanController;
                }
                else
                {
                    Log.WriteToLog($"No match: {ttFanController.Name} has product ids ranging from {ttFanController.ProductIdStart} to {ttFanController.ProductIdEnd}");
                }
            }

            return null;
        }

        public void Disconnect()
        {
            foreach (HidSharp.HidStream stream in this.openStreams)
            {
                try { stream.Dispose(); } catch { }
            }
            this.openStreams.Clear();
            this.Devices.Clear();
            this.isConnected = false;
        }

        public List<TTFanControllerInterface> GetFanControllers()
        {
            return this.Devices;
        }
    }
}
