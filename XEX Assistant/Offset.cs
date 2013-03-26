using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDevkit;

namespace XEX_Assistant
{
    public class Offset
    {
        public string Address { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public Offset(string offset, string type)
        {
            Address = offset;
            Type = type;
            RealTimeEditing rte = new RealTimeEditing(new XboxManager().DefaultConsole);
            Value = rte.PeekXbox(Convert.ToUInt32(Address, 0x10), type, false);
        }
    }
}
