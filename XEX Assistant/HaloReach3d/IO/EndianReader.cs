namespace HaloReach3d.IO
{
    using System;
    using System.IO;

    public class EndianReader : BinaryReader
    {
        public EndianType endianstyle;

        public EndianReader(Stream stream, EndianType endianstyle) : base(stream)
        {
            this.endianstyle = endianstyle;
        }

        public string ReadAsciiString(int Length)
        {
            return this.ReadAsciiString(Length, this.endianstyle);
        }

        public string ReadAsciiString(int Length, EndianType EndianType)
        {
            string newString = "";
            int howMuch = 0;
            for (int x = 0; x < Length; x++)
            {
                char tempChar = (char) this.ReadByte();
                howMuch++;
                if (tempChar == '\0')
                {
                    break;
                }
                newString = newString + tempChar;
            }
            int size = Length - howMuch;
            this.BaseStream.Seek((long) size, SeekOrigin.Current);
            return newString;
        }

        public override double ReadDouble()
        {
            return this.ReadDouble(this.endianstyle);
        }

        public double ReadDouble(EndianType EndianType)
        {
            byte[] buffer = base.ReadBytes(8);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            return BitConverter.ToDouble(buffer, 0);
        }

        public override short ReadInt16()
        {
            return this.ReadInt16(this.endianstyle);
        }

        public short ReadInt16(EndianType EndianType)
        {
            byte[] buffer = base.ReadBytes(2);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            return BitConverter.ToInt16(buffer, 0);
        }

        public int ReadInt24()
        {
            return this.ReadInt24(this.endianstyle);
        }

        public int ReadInt24(EndianType EndianType)
        {
            byte[] buffer = base.ReadBytes(3);
            byte[] dest = new byte[4];
            Array.Copy(buffer, 0, dest, 0, 3);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(dest);
            }
            return BitConverter.ToInt32(dest, 0);
        }

        public override int ReadInt32()
        {
            return this.ReadInt32(this.endianstyle);
        }

        public int ReadInt32(EndianType EndianType)
        {
            byte[] buffer = base.ReadBytes(4);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            return BitConverter.ToInt32(buffer, 0);
        }

        public override long ReadInt64()
        {
            return this.ReadInt64(this.endianstyle);
        }

        public long ReadInt64(EndianType EndianType)
        {
            byte[] buffer = base.ReadBytes(8);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            return BitConverter.ToInt64(buffer, 0);
        }

        public string ReadNullTerminatedString()
        {
            char temp;
            string newString = "";
            while ((temp = this.ReadChar()) != '\0')
            {
                if (temp == '\0')
                {
                    return newString;
                }
                newString = newString + temp;
            }
            return newString;
        }

        public override float ReadSingle()
        {
            return this.ReadSingle(this.endianstyle);
        }

        public float ReadSingle(EndianType EndianType)
        {
            byte[] buffer = base.ReadBytes(4);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            return BitConverter.ToSingle(buffer, 0);
        }

        public override string ReadString()
        {
            string newString = "";
            int howMuch = 0;
            while (true)
            {
                char tempChar = (char) this.ReadByte();
                howMuch++;
                if (tempChar == '\0')
                {
                    break;
                }
                newString = newString + tempChar;
            }
            int size = newString.Length - howMuch;
            this.BaseStream.Seek((long) (size + 1), SeekOrigin.Current);
            return newString;
        }

        public string ReadString(int Length)
        {
            return this.ReadAsciiString(Length);
        }

        public override ushort ReadUInt16()
        {
            return this.ReadUInt16(this.endianstyle);
        }

        public ushort ReadUInt16(EndianType EndianType)
        {
            byte[] buffer = base.ReadBytes(2);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            return BitConverter.ToUInt16(buffer, 0);
        }

        public override uint ReadUInt32()
        {
            return this.ReadUInt32(this.endianstyle);
        }

        public uint ReadUInt32(EndianType EndianType)
        {
            byte[] buffer = base.ReadBytes(4);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            return BitConverter.ToUInt32(buffer, 0);
        }

        public override ulong ReadUInt64()
        {
            return this.ReadUInt64(this.endianstyle);
        }

        public ulong ReadUInt64(EndianType EndianType)
        {
            byte[] buffer = base.ReadBytes(8);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            return BitConverter.ToUInt64(buffer, 0);
        }

        public string ReadUnicodeString(int Length)
        {
            return this.ReadUnicodeString(Length, this.endianstyle);
        }

        public string ReadUnicodeString(int Length, EndianType EndianType)
        {
            string newString = "";
            int howMuch = 0;
            for (int x = 0; x < Length; x++)
            {
                char tempChar = (char) this.ReadUInt16(EndianType);
                howMuch++;
                if (tempChar == '\0')
                {
                    break;
                }
                newString = newString + tempChar;
            }
            int size = (Length - howMuch) * 2;
            this.BaseStream.Seek((long) size, SeekOrigin.Current);
            return newString;
        }

        public void SeekTo(int offset)
        {
            this.SeekTo(offset, SeekOrigin.Begin);
        }

        public void SeekTo(long offset)
        {
            this.SeekTo((int) offset, SeekOrigin.Begin);
        }

        public void SeekTo(uint offset)
        {
            this.SeekTo((int) offset, SeekOrigin.Begin);
        }

        public void SeekTo(int offset, SeekOrigin SeekOrigin)
        {
            this.BaseStream.Seek((long) offset, SeekOrigin);
        }
    }
}

