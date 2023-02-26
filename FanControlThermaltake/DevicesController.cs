using System;
using System.Collections.Generic;

namespace FanControl.ThermaltakeRiingPlus
{
    public class DevicesController
    {
        protected int VendorId = 0x264a;
        
        protected int MaxConnectedDevices = 5;
        protected bool isConnected = false;
        protected List<TTFanControllerInterface> Devices = new List<TTFanControllerInterface>();
        public void Connect()
        {
            if (!this.isConnected)
            {
                
                // Get devices from HID
                IEnumerable<HidSharp.HidDevice> deviceList = HidSharp.DeviceList.Local.GetHidDevices();
                //HidDeviceList = HidDevices.Enumerate(this.VendorId);
                foreach (HidSharp.HidDevice hidDevice in deviceList)
                {
                    if (hidDevice.VendorID == this.VendorId)
                    {
                        Log.WriteToLog($"Found Thermaltake device with Vendor ID {hidDevice.VendorID} and Product ID {hidDevice.ProductID}");
                        var fanControllerMatch = this.findController(hidDevice.ProductID);
                        if (fanControllerMatch)
                        {
                            
                            Log.WriteToLog("We found a TT device");

                            if (hidDevice.TryOpen(out HidSharp.HidStream hidStream))
                            {
                                Log.WriteToLog("We opened the HID Device");
                                int controllerIndex = this.Devices.Count;
                                TTFanControllerInterface ttFanController = Activator.CreateInstance(fanControllerMatch, hidStream, controllerIndex) as TTFanControllerInterface;

                                Log.WriteToLog("Adding HID Device to Devices");
                                this.Devices.Add(ttFanController);
                                Log.WriteToLog($"We have {this.Devices.Count} devices");
                            }
                        }
                        else
                        {
                            Log.WriteToLog("We found a TT device, but it isn't supported yet. Please post an issue here (https://github.com/fu-raz/FanControlThermaltake/issues/) with this log");
                        }
                    }

                }
            }

        }

        private findController(int hidDeviceProductId)
        {
            string targetNamespace = "FanControl.ThermaltakeRiingPlus.FanControllers";
            // Get all types in the current assembly
            var matchingTypes = Assembly.GetExecutingAssembly()
                                    .GetTypes()
                                    .Where(t => t.Namespace == targetNamespace &&
                                                t.GetProperties(BindingFlags.Static | BindingFlags.Public)
                                                .Any(p => p.Name == "ProductIdStart" || p.Name == "ProductIdEnd")
                                                );

            foreach (var match in matchingTypes)
            {
                PropertyInfo productIdStart = match.GetProperty("ProductIdStart", BindingFlags.Static | BindingFlags.Public);
                PropertyInfo productIdEnd = match.GetProperty("ProductIdEnd", BindingFlags.Static | BindingFlags.Public);
                if (productIdStart != null && productIdEnd != null)
                {
                    int start = (int)productIdStart.GetValue(null);
                    int end = (int)productIdEnd.GetValue(null);
                    if (hidDeviceProductId >= start && hidDeviceProductId <= end)
                    {
                        return match;
                    }
                }
            }

            return false;
        }
        public void Disconnect()
        {
            // I dunno what to do
        }

        public List<TTFanController> GetFanControllers()
        {
            return this.Devices;
        }

    }
}
