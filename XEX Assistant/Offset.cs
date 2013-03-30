using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDevkit;
using HaloDevelopmentExtender;
using HaloReach3d;

namespace XEX_Assistant
{
    public class Offset
    {
        public uint Address { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public Offset(uint offset, string type, bool getValueFromXbox)
        {
            Address = offset;
            Type = type;
            if (getValueFromXbox)
            {
                Value = getValue(Address, Type);
            }
        }

        public Offset(uint offset, string type, string value)
        {
            Address = offset;
            Type = type;
            Value = value;
        }

        public string getValue(uint offset, string type)
        {
            string hex = "X";
            object rn = null;
                XboxDebugCommunicator Xbox_Debug_Communicator = new XboxDebugCommunicator(new XboxManager().DefaultConsole);
                //Connect
                if (Xbox_Debug_Communicator.Connected == false)
                {
                    try
                    {
                        Xbox_Debug_Communicator.Connect();
                    }
                    catch { }
                }

                XboxMemoryStream xbms = Xbox_Debug_Communicator.ReturnXboxMemoryStream();
                //Endian IO
                HaloReach3d.IO.EndianIO IO = new HaloReach3d.IO.EndianIO(xbms,
                    HaloReach3d.IO.EndianType.BigEndian);
                IO.Open();
                IO.In.BaseStream.Position = offset;

                if (type == "String" | type == "string")
                    rn = IO.In.ReadString();
                if (type == "Float" | type == "float")
                    rn = IO.In.ReadSingle();
                if (type == "Double" | type == "double")
                    rn = IO.In.ReadDouble();
                if (type == "Short" | type == "short")
                    rn = IO.In.ReadInt16().ToString(hex);
                if (type == "Byte" | type == "byte")
                    rn = IO.In.ReadByte().ToString(hex);
                if (type == "Long" | type == "long")
                    rn = IO.In.ReadInt32().ToString(hex);
                if (type == "Quad" | type == "quad")
                    rn = IO.In.ReadInt64().ToString(hex);

                IO.Close();
                xbms.Close();
                Xbox_Debug_Communicator.Disconnect();

                return rn.ToString();
        }
    }
}
