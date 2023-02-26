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
        public new string Name => "Riing Quad";
        public new int PortCount = 5;

        public new int ProductIdStart => 0x2260;
        public new int ProductIdEnd => 0x2270;

    }
}
