using System;
using System.Collections.Generic;

namespace FanControl.ThermaltakeRiingPlus
{
    public class DevicesController
    {
        protected int VendorId = 0x264a;
        protected int ProductId = 0x2260;
        protected int MaxConnectedDevices = 5;
        protected bool isConnected = false;
        protected List<TTFanController> Devices = new List<TTFanController>();
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
                        
                        if (hidDevice.ProductID >= this.ProductId &&
                        hidDevice.ProductID <= this.ProductId + this.MaxConnectedDevices)
                        {
                            Log.WriteToLog("We found a TT device");
                            
                            if (hidDevice.TryOpen(out HidSharp.HidStream hidStream))
                            {
                                Log.WriteToLog("We opened the HID Device");
                                int controllerIndex = this.Devices.Count;
                                TTFanController ttFanController = new TTFanController(hidStream, controllerIndex);

                                Log.WriteToLog("Adding HID Device to Devices");
                                this.Devices.Add(ttFanController);
                                Log.WriteToLog($"We have {this.Devices.Count} devices");
                            }

                        }
                    }

                }
            }

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
