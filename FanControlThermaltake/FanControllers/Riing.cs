using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.ThermaltakeRiingPlus.FanControllers
{
    public class Riing : TTFanController, TTFanControllerInterface
    {
        public new string Name => "Riing";
        public new int PortCount = 5;

        public new int ProductIdStart => 0x1f41;
        public new int ProductIdEnd => 0x1f51;

        public new byte byteSetSpeed => 0x03;
    }
}
