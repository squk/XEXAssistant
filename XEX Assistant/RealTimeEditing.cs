using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDevkit;
using HaloDevelopmentExtender;
using HaloReach3d;
using System.Windows;
using Elysium.Notifications;
using System.Windows.Media;

namespace XEX_Assistant
{
    class RealTimeEditing
    {
        private string xdkName;
        public bool isConnected;

        private XboxDebugCommunicator Xbox_Debug_Communicator;

        public RealTimeEditing()
        {
            isConnected = false;
        }

        public RealTimeEditing(string XdkName)
        {
            xdkName = XdkName;
            Elysium.Manager.Apply(Application.Current, Elysium.Theme.Light, Brushes.BlueViolet, Brushes.Black);
            if (xdkName == "")
            {
                Elysium.Notifications.NotificationManager.BeginTryPush("Error", "XDKName/IP not set. ");
            }
        }

        public bool Connect()
        {
            //Elysium.Manager.Apply(Application.Current, Elysium.Theme.Light, Brushes.BlueViolet, Brushes.Black);
            Xbox_Debug_Communicator = new XboxDebugCommunicator(xdkName);
            if (!Xbox_Debug_Communicator.Connected)
            {
                try
                {
                    Xbox_Debug_Communicator.Connect();
                    isConnected = true;
                    Elysium.Notifications.NotificationManager.BeginTryPush("Success", "You are now connected to " + xdkName);
                    return true;
                }
                catch
                {
                    isConnected = false;
                    Elysium.Notifications.NotificationManager.BeginTryPush("Error", "Connection to " + xdkName + " failed. ");
                    return false;
                }
            }
            else
            {
                isConnected = true;
                Elysium.Notifications.NotificationManager.BeginTryPush("Success", "You are already connected to " + xdkName);
                return true;
            }
        }
           
        public bool ConnectWithoutNotification()
        {
            //Elysium.Manager.Apply(Application.Current, Elysium.Theme.Light, Brushes.BlueViolet, Brushes.Black);
            Xbox_Debug_Communicator = new XboxDebugCommunicator(xdkName);
            if (!Xbox_Debug_Communicator.Connected)
            {
                try
                {
                    Xbox_Debug_Communicator.Connect();
                    isConnected = true;
                    return true;
                }
                catch
                {
                    isConnected = false;
                    return false;
                }
            }
            else
            {
                isConnected = true;
                return true;
            }
        }

        public void Disconnect()
        {
            Xbox_Debug_Communicator.Disconnect();
            isConnected = false;
            Elysium.Notifications.NotificationManager.BeginTryPush("Success", "You are no longer connected to " + xdkName);
        }

        public void DisconnectWithoutNotification()
        {
            Xbox_Debug_Communicator.Disconnect();
            isConnected = false;
        }

        public void PokeXbox(Offset offsetData)
        {
            uint offset = Convert.ToUInt32(offsetData.Address, 0x10);
            string poketype = offsetData.Type;
            string amount = offsetData.Value;

            if (isConnected)
            {
                try
                {
                    if (!Xbox_Debug_Communicator.Connected)
                    {
                        try
                        {
                            Xbox_Debug_Communicator.Connect();
                        }
                        catch
                        {

                        }
                    }
                    XboxMemoryStream xbms = Xbox_Debug_Communicator.ReturnXboxMemoryStream();
                    HaloReach3d.IO.EndianIO IO = new HaloReach3d.IO.EndianIO(xbms, HaloReach3d.IO.EndianType.BigEndian);
                    IO.Open();
                    IO.Out.BaseStream.Position = offset;
                    if (poketype == "Unicode String")
                    {
                        IO.Out.WriteUnicodeString(amount, amount.Length);
                    }
                    if (poketype == "ASCII String")
                    {
                        IO.Out.WriteUnicodeString(amount, amount.Length);
                    }
                    if ((poketype == "String") | (poketype == "string"))
                    {
                        IO.Out.Write(amount);
                    }
                    if ((poketype == "Float") | (poketype == "float"))
                    {
                        IO.Out.Write(float.Parse(amount));
                    }
                    if ((poketype == "Double") | (poketype == "double"))
                    {
                        IO.Out.Write(double.Parse(amount));
                    }
                    if ((poketype == "Short") | (poketype == "short"))
                    {
                        IO.Out.Write((short)Convert.ToUInt32(amount, 0x10));
                    }
                    if ((poketype == "Byte") | (poketype == "byte"))
                    {
                        IO.Out.Write((byte)Convert.ToUInt32(amount, 0x10));
                    }
                    if ((poketype == "Long") | (poketype == "long"))
                    {
                        IO.Out.Write((long)Convert.ToUInt32(amount, 0x10));
                    }
                    if ((poketype == "Quad") | (poketype == "quad"))
                    {
                        IO.Out.Write((long)Convert.ToUInt64(amount, 0x10));
                    }
                    if ((poketype == "Int") | (poketype == "int"))
                    {
                        IO.Out.Write(Convert.ToUInt32(amount, 0x10));
                    }
                    IO.Close();
                    xbms.Close();
                    Elysium.Notifications.NotificationManager.BeginTryPush("Poked", "Poked " + amount + " to 0x" + offset.ToString("X"));
                }
                catch
                {
                    Elysium.Notifications.NotificationManager.BeginTryPush("Error", "Couldn't poke XDK. ");
                }
            }
            else
            {
                Elysium.Notifications.NotificationManager.BeginTryPush("Error", "You are not connected to your XDK. ");
            }
        }

        public string PeekXbox(uint offset, string type)
        {
            if (isConnected)
            {
                string hex = "X";
                object rn = null;
                if (xdkName != "")
                {
                    if (!Xbox_Debug_Communicator.Connected)
                    {
                        try
                        {
                            Xbox_Debug_Communicator.Connect();
                        }
                        catch
                        {
                        }
                    }
                    XboxMemoryStream xbms = Xbox_Debug_Communicator.ReturnXboxMemoryStream();
                    HaloReach3d.IO.EndianIO IO = new HaloReach3d.IO.EndianIO(xbms, HaloReach3d.IO.EndianType.BigEndian);
                    IO.Open();
                    IO.In.BaseStream.Position = offset;
                    if ((type == "String") | (type == "string"))
                    {
                        rn = IO.In.ReadString();
                    }
                    if ((type == "Float") | (type == "float"))
                    {
                        rn = IO.In.ReadSingle();
                    }
                    if ((type == "Double") | (type == "double"))
                    {
                        rn = IO.In.ReadDouble();
                    }
                    if ((type == "Short") | (type == "short"))
                    {
                        rn = IO.In.ReadInt16().ToString(hex);
                    }
                    if ((type == "Byte") | (type == "byte"))
                    {
                        rn = IO.In.ReadByte().ToString(hex);
                    }
                    if ((type == "Long") | (type == "long"))
                    {
                        rn = IO.In.ReadInt32().ToString(hex);
                    }
                    if ((type == "Quad") | (type == "quad"))
                    {
                        rn = IO.In.ReadInt64().ToString(hex);
                    }
                    IO.Close();
                    xbms.Close();
                    return rn.ToString();
                }
                MessageBox.Show("XDK Name/IP not set");
                return "No Console Detected";
            }
            else
            {
                Elysium.Notifications.NotificationManager.BeginTryPush("Error", "You are not connected to your XDK");
                return "Not Connected";
            }
        }
    }
}
