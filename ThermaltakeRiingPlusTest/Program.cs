
using System;
using System.Collections.Generic;

namespace ThermaltakeRiingPlusTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int VendorId = 0x264a;
            int ProductId = 0x2260;
            int MaxConnectedDevices = 5;

            int TTDevices = 0;
            int SupportedDevices = 0;
            int UnsupportedDevices = 0;

            // Get devices from HID
            IEnumerable<HidSharp.HidDevice> deviceList = HidSharp.DeviceList.Local.GetHidDevices();
            //HidDeviceList = HidDevices.Enumerate(this.VendorId);
            foreach (HidSharp.HidDevice hidDevice in deviceList)
            {
                if (hidDevice.VendorID == VendorId)
                {
                    Console.WriteLine($"Found Thermaltake device with Vendor ID {hidDevice.VendorID} and Product ID {hidDevice.ProductID}");
                    TTDevices++;

                    if (hidDevice.ProductID >= ProductId &&
                    hidDevice.ProductID <= ProductId + MaxConnectedDevices)
                    {
                        SupportedDevices++;
                    }
                    else
                    {
                        Console.WriteLine($"We found an unsupported TT device with product ID {hidDevice.ProductID}");
                        UnsupportedDevices++;
                    }
                }
            }

            Console.WriteLine($"We found {TTDevices} Thermaltake devices, {SupportedDevices} supported and {UnsupportedDevices} unsupported");
            if (UnsupportedDevices > 0)
            {
                Console.WriteLine($"Please raise an issue here with the unsupported product ID's: https://github.com/fu-raz/FanControlThermaltake/issues");
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

    }
}