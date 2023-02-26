using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.ThermaltakeRiingPlus
{
    public interface TTFanControllerInterface
    {
        public string Name;
        public int PortCount;
        public static byte ProductIdStart;
        public static byte ProductIdEnd;
        public int GetFanRPM(int portNumber);
        public int GetFanPower(int portNumber);
        public void SetFanPower(int portNumber, float value);
        public List<ControlSensor> GetControlSensors();
        public List<FanSensor> GetFanSensors();
    }
}
