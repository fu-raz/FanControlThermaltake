using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.Thermaltake.FanControllers
{
    public class Riing : TTFanController, TTFanControllerInterface
    {
        public override string Name => "Riing";
        public override int PortCount => 5;

        public override int ProductIdStart => 0x1f41;
        public override int ProductIdEnd => 0x1f51;

        public override byte byteSetSpeed => 0x03;
    }
}
