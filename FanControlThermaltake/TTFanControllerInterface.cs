using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.ThermaltakeRiingPlus
{
    public interface TTFanControllerInterface
    {
        string Name { get; }
        int PortCount { get; }
        int ProductIdStart { get; }
        int ProductIdEnd { get; }
        void init(HidSharp.HidStream hidDevice, int index);
        int GetFanRPM(int portNumber);
        int GetFanPower(int portNumber);
        void SetFanPower(int portNumber, float value);
        List<ControlSensor> GetControlSensors();
        List<FanSensor> GetFanSensors();
    }
}
