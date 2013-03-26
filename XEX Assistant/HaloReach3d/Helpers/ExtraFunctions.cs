
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using HaloReach3d.IO;

namespace HaloReach3d.Helpers
{
    public abstract class ExtraFunctions
    {
        public static string BytesToString(byte[] data)
        {
            return RemoveWhiteSpacingFromString(Encoding.ASCII.GetString(data));
        }
        public static byte[] StringToBytes(string data)
        {
            return Encoding.ASCII.GetBytes(data);
        }
        public static string BytesToHexString(byte[] data)
        {
            string hexString = "";
            for (int i = 0; i < data.Length; i++)
            {
                string dataStr = data[i].ToString("X");
                while (dataStr.Length != 2)
                {
                    dataStr = "0" + dataStr;
                }
                hexString += dataStr;
            }
            return hexString;
        }
        public static byte[] HexStringToBytes(string hexString)
        {
            List<byte> data = new List<byte>();
            for (int i = 0; i < hexString.Length; i += 2)
            {
                if (hexString.Length > i + 1)
                {
                    data.Add(byte.Parse(hexString[i] + hexString[i + 1].ToString(), System.Globalization.NumberStyles.HexNumber));
                }
            }
            return data.ToArray();
        }
        public static string RemoveWhiteSpacingFromString(string str)
        {
            return str.Replace("\0", "");
        }
        public static byte[] GetBytesFromStream(BinaryReader br)
        {
            return GetBytesFromStream(br, 0, (int)br.BaseStream.Length);
        }
        public static byte[] GetBytesFromStream(BinaryReader br, int offset, int size)
        {
            br.BaseStream.Position = offset;
            byte[] data = br.ReadBytes(size);
            return data;
        }
        public static byte[] StringToUnicodeBytes(string text)
        {
            byte[] origData = StringToBytes(text);
            byte[] newData = new byte[origData.Length * 2];
            for (int i = 0; i < origData.Length; i++)
            {
                newData[i * 2] = origData[i];
            }
            return newData;
        }
        public static byte[] StringToUnicodeBytes(string text, int Length)
        {
            byte[] origData = StringToBytes(text);
            byte[] newData = new byte[Length];
            for (int i = 0; i < origData.Length; i++)
                newData[i * 2] = origData[i];
            return newData;
        }
        public static void SwapBytes(ref byte[] data)
        {
            Array.Reverse(data);
        }
        public static byte[] ValueToBytes(string data, bool unicode)
        {
            if (unicode)
            {
                return StringToUnicodeBytes(data);
            }
            return StringToBytes(data);
        }
        public static byte[] ValueToBytes(byte data)
        {
            return new byte[] { data };
        }
        public static byte[] ValueToBytes(short data)
        {
            MemoryStream ms = new MemoryStream();
            EndianIO IO = new EndianIO(ms, EndianType.BigEndian);
            IO.Open();
            IO.Out.Write(data);
            IO.In.BaseStream.Position = 0;
            byte[] array = IO.In.ReadBytes((int)IO.In.BaseStream.Length);
            IO.Close();
            ms.Close();
            return array;
        }
        public static byte[] ValueToBytes(ushort data)
        {
            MemoryStream ms = new MemoryStream();
            EndianIO IO = new EndianIO(ms, EndianType.BigEndian);
            IO.Open();
            IO.Out.Write(data);
            IO.In.BaseStream.Position = 0;
            byte[] array = IO.In.ReadBytes((int)IO.In.BaseStream.Length);
            IO.Close();
            ms.Close();
            return array;
        }
        public static byte[] ValueToBytes(int data)
        {
            MemoryStream ms = new MemoryStream();
            EndianIO IO = new EndianIO(ms, EndianType.BigEndian);
            IO.Open();
            IO.Out.Write(data);
            IO.In.BaseStream.Position = 0;
            byte[] array = IO.In.ReadBytes((int)IO.In.BaseStream.Length);
            IO.Close();
            ms.Close();
            return array;
        }
        public static byte[] ValueToBytes(uint data)
        {
            MemoryStream ms = new MemoryStream();
            EndianIO IO = new EndianIO(ms, EndianType.BigEndian);
            IO.Open();
            IO.Out.Write(data);
            IO.In.BaseStream.Position = 0;
            byte[] array = IO.In.ReadBytes((int)IO.In.BaseStream.Length);
            IO.Close();
            ms.Close();
            return array;
        }
        public static byte[] ValueToBytes(float data)
        {
            MemoryStream ms = new MemoryStream();
            EndianIO IO = new EndianIO(ms, EndianType.BigEndian);
            IO.Open();
            IO.Out.Write(data);
            IO.In.BaseStream.Position = 0;
            byte[] array = IO.In.ReadBytes((int)IO.In.BaseStream.Length);
            IO.Close();
            ms.Close();
            return array;
        }
        public static int CalculatePadding(int integer, int interval)
        {
            return interval - (integer % interval);
        }
        /// <summary>
        /// This function will insert bytes into the given file.
        /// </summary>
        /// <param name="filePath">The file to insert bytes into.</param>
        /// <param name="startOffset">The offset where we should start inserting</param>
        /// <param name="size">How many bytes we should insert.</param>
        public static void InsertBytes(string filePath, int startOffset, int size)
        {
            //Create our filestream
            FileStream fs = new FileStream(filePath, FileMode.Open);

            //Create our binary Reader
            BinaryReader br = new BinaryReader(fs);

            //Go to our position
            br.BaseStream.Position = startOffset;

            //Read the second part of our file
            byte[] secondArray = br.ReadBytes((int)br.BaseStream.Length - (int)br.BaseStream.Position);

            //Close our IO
            br.Close();
            fs.Close();

            //Recreate our filestream
            fs = new FileStream(filePath, FileMode.Open);

            //Create our binary writer
            BinaryWriter bw = new BinaryWriter(fs);

            //Go to our position
            bw.BaseStream.Position = startOffset;

            //Write our blank bytes
            bw.Write(new byte[size]);

            //Write our second part of the file
            bw.Write(secondArray);

            //Close our IO
            fs.Close();
            bw.Close();
        }
        /// <summary>
        /// This function will insert bytes into the given file.
        /// </summary>
        /// <param name="filePath">The file to insert bytes into.</param>
        /// <param name="startOffset">The offset where we should start inserting</param>
        /// <param name="size">How many bytes we should insert.</param>
        public static void InsertBytes(string filePath, int startOffset, byte[] data)
        {
            //Create our filestream
            FileStream fs = new FileStream(filePath, FileMode.Open);

            //Create our binary Reader
            BinaryReader br = new BinaryReader(fs);

            //Go to our position
            br.BaseStream.Position = startOffset;

            //Read the second part of our file
            byte[] secondArray = br.ReadBytes((int)br.BaseStream.Length - (int)br.BaseStream.Position);

            //Close our IO
            br.Close();
            fs.Close();

            //Recreate our filestream
            fs = new FileStream(filePath, FileMode.Open);

            //Create our binary writer
            BinaryWriter bw = new BinaryWriter(fs);

            //Go to our position
            bw.BaseStream.Position = startOffset;

            //Write our blank bytes
            bw.Write(data);

            //Write our second part of the file
            bw.Write(secondArray);

            //Close our IO
            fs.Close();
            bw.Close();
        }
        /// <summary>
        /// This function will delete bytes from a given file.
        /// </summary>
        /// <param name="filePath">The file to delete bytes from.</param>
        /// <param name="startOffset">The offset where we should start deleting</param>
        /// <param name="size">How many bytes we should delete</param>
        public static void DeleteBytes(string filePath, int startOffset, int size)
        {
            //Create our filestream
            FileStream fs = new FileStream(filePath, FileMode.Open);

            //Create our binary Reader
            BinaryReader br = new BinaryReader(fs);

            //Read our first array
            byte[] firstArray = br.ReadBytes(startOffset);

            //Skip our data
            br.BaseStream.Position += size;

            //Read the second part of our file
            byte[] secondArray = br.ReadBytes((int)br.BaseStream.Length - (int)br.BaseStream.Position);

            //Close our IO
            br.Close();
            fs.Close();

            //Recreate our filestream
            fs = new FileStream(filePath, FileMode.Create);

            //Create our binary writer
            BinaryWriter bw = new BinaryWriter(fs);

            //Write our first part of our file
            bw.Write(firstArray);

            //Write our second part of the file
            bw.Write(secondArray);

            //Close our IO
            fs.Close();
            bw.Close();
        }
    }
}
