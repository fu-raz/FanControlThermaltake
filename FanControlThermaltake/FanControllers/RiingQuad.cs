using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.ThermaltakeRiingPlus.FanControllers
{
    public class RiingQuad : TTFanController, TTFanControllerInterface
    {
        public override string Name => "Riing Quad";
        public override int PortCount => 5;

        public override int ProductIdStart => 0x2260;
        public override int ProductIdEnd => 0x2270;

    }
}
