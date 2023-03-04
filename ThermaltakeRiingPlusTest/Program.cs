
using FanControl.Thermaltake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ThermaltakeRiingPlusTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int vendorId = 0x264a;

            IEnumerable<HidSharp.HidDevice> deviceList = HidSharp.DeviceList.Local.GetHidDevices();
            //HidDeviceList = HidDevices.Enumerate(this.VendorId);
            foreach (HidSharp.HidDevice hidDevice in deviceList)
            {
                if (hidDevice.VendorID == vendorId)
                {
                    int productId = hidDevice.ProductID;

                    string targetNamespace = "FanControl.Thermaltake.FanControllers";

                    var assembly = Assembly.GetAssembly(typeof(FanControl.Thermaltake.FanControllers.Riing));
                    var matchingTypes = assembly.GetTypes().Where(t => t.Namespace == targetNamespace);

                    bool found = false;

                    foreach (var match in matchingTypes)
                    {
                        TTFanControllerInterface ttFanController = Activator.CreateInstance(match) as TTFanControllerInterface;
                        if (productId >= ttFanController.ProductIdStart && productId <= ttFanController.ProductIdEnd)
                        {
                            found = true;
                            if (hidDevice.TryOpen(out HidSharp.HidStream hidStream))
                            {
                                ttFanController.init(hidStream, 1, productId);
                                Console.WriteLine("---------------------");
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"We found a controller {ttFanController.Name}");
                                Console.ResetColor();

                                List<ControlSensor> controlSensors = ttFanController.GetControlSensors();
                                List<FanSensor> fanSensors = ttFanController.GetFanSensors();

                                Console.WriteLine($"We detected {controlSensors.Count} control sensors and {fanSensors.Count} fan sensors");
                                foreach (var sensor in fanSensors)
                                {
                                    sensor.Update();
                                    Console.WriteLine($"Fan {sensor.portNumber} is running at {sensor.Value} RPM");
                                }
                            }

                            break;
                        }
                    }

                    if (!found)
                    {
                        Console.WriteLine($"We found a TT controller, but we don't yet support this one. Please raise an issue here https://github.com/fu-raz/FanControlThermaltake/issues with the following product id 0x{productId:X}");
                    }
                }
            }
        }

    }
}