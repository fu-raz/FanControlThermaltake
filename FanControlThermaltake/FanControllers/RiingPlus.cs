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
        public new string Name => "Riing Plus";
        public new int PortCount = 5;

        public new int ProductIdStart => 0x1fa5;
        public new int ProductIdEnd => 0x1fb5;

    }
}
