using System.Collections.Generic;

namespace FanControl.Thermaltake
{
    public interface TTFanControllerInterface
    {
        string Name { get; }
        int PortCount { get; }
        int ProductIdStart { get; }
        int ProductIdEnd { get; }

        byte byteGet { get; }
        byte byteGetSpeed { get; }
        byte byteSet { get; }
        byte byteSetSpeed { get; }
        byte byteInit { get; }

        void init(HidSharp.HidStream hidDevice, int index, int productId);
        void PollAllPorts();
        int GetFanRPM(int portNumber);
        int GetFanPower(int portNumber);
        void SetFanPower(int portNumber, float value);
        List<ControlSensor> GetControlSensors();
        List<FanSensor> GetFanSensors();
    }
}
