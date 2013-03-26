namespace HaloReach3d.IO
{
    using System;
    using System.IO;

    public class EndianWriter : BinaryWriter
    {
        private readonly EndianType endianstyle;

        public EndianWriter(Stream stream, EndianType endianstyle) : base(stream)
        {
            this.endianstyle = endianstyle;
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

        public override void Write(double value)
        {
            this.Write(value, this.endianstyle);
        }

        public override void Write(short value)
        {
            this.Write(value, this.endianstyle);
        }

        public override void Write(int value)
        {
            this.Write(value, this.endianstyle);
        }

        public override void Write(long value)
        {
            this.Write(value, this.endianstyle);
        }

        public override void Write(float value)
        {
            this.Write(value, this.endianstyle);
        }

        public override void Write(ushort value)
        {
            this.Write(value, this.endianstyle);
        }

        public override void Write(uint value)
        {
            this.Write(value, this.endianstyle);
        }

        public override void Write(ulong value)
        {
            this.Write(value, this.endianstyle);
        }

        public void Write(double value, EndianType EndianType)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            base.Write(buffer);
        }

        public void Write(short value, EndianType EndianType)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            base.Write(buffer);
        }

        public void Write(int value, EndianType EndianType)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            base.Write(buffer);
        }

        public void Write(long value, EndianType EndianType)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            base.Write(buffer);
        }

        public void Write(float value, EndianType EndianType)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            base.Write(buffer);
        }

        public void Write(ushort value, EndianType EndianType)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            base.Write(buffer);
        }

        public void Write(uint value, EndianType EndianType)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            base.Write(buffer);
        }

        public void Write(ulong value, EndianType EndianType)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (EndianType == EndianType.BigEndian)
            {
                Array.Reverse(buffer);
            }
            base.Write(buffer);
        }

        public void WriteAsciiString(string String, int Length)
        {
            this.WriteAsciiString(String, Length, this.endianstyle);
        }

        public void WriteAsciiString(string String, int Length, EndianType EndianType)
        {
            int strLen = String.Length;
            for (int x = 0; x < strLen; x++)
            {
                if (x > Length)
                {
                    break;
                }
                byte val = (byte) String[x];
                this.Write(val);
            }
            int nullSize = Length - strLen;
            if (nullSize > 0)
            {
                this.Write(new byte[nullSize]);
            }
        }

        public void WriteUnicodeString(string String, int Length)
        {
            this.WriteUnicodeString(String, Length, this.endianstyle);
        }

        public void WriteUnicodeString(string String, int Length, EndianType EndianType)
        {
            int strLen = String.Length;
            for (int x = 0; x < strLen; x++)
            {
                if (x > Length)
                {
                    break;
                }
                ushort val = String[x];
                this.Write(val, EndianType);
            }
            int nullSize = (Length - strLen) * 2;
            if (nullSize > 0)
            {
                this.Write(new byte[nullSize]);
            }
        }
    }
}

