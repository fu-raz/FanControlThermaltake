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

        public new static byte ProductIdStart => 0x1f41;
        public new static byte ProductIdEnd => 0x1f51;

        public RiingPlus(HidStream hidDevice, int index) : base(hidDevice, index)
        {
        }
    }
}
