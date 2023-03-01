using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.ThermaltakeRiingPlus.FanControllers
{
    public class RiingPlus : TTFanController, TTFanControllerInterface
    {
        public override string Name => "Riing Plus";
        public override int PortCount => 5;

        public override int ProductIdStart => 0x1fa5;
        public override int ProductIdEnd => 0x1fb5;

    }
}
