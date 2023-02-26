using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.ThermaltakeRiingPlus.FanControllers
{
    public class RiingTrio : TTFanController, TTFanControllerInterface
    {
        public new string Name => "Riing Trio";
        public new int PortCount = 5;

        public new int ProductIdStart => 0x2135;
        public new int ProductIdEnd => 0x2145;

    }
}
