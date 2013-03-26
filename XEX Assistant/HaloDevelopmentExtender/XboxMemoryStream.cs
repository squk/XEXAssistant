namespace HaloDevelopmentExtender
{
    using System;
    using System.IO;
    using XDevkit;

    public class XboxMemoryStream : Stream
    {
        protected uint position;
        private readonly IXboxDebugTarget target;

        public XboxMemoryStream(IXboxDebugTarget Target)
        {
            this.target = Target;
            this.position = 0x10114;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            uint read;
            if (offset == 0)
            {
                this.target.GetMemory(this.position, (uint) count, buffer, out read);
            }
            else
            {
                byte[] temp = new byte[count];
                this.target.GetMemory(this.position, (uint) count, temp, out read);
                temp.CopyTo(buffer, offset);
            }
            this.position += read;
            return (int) read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    return (long) (this.position = (uint) offset);

                case SeekOrigin.Current:
                    return (long) (this.position += ((uint) offset));

                case SeekOrigin.End:
                    return (long) (this.position -= ((uint) offset));
            }
            throw new Exception("Invalid SeekOrigin.");
        }

        public override void SetLength(long value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            uint bytesWritten;
            this.target.SetMemory(this.position, (uint) count, buffer, out bytesWritten);
            this.position += bytesWritten;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public override long Position
        {
            get
            {
                return (long) this.position;
            }
            set
            {
                this.position = (uint) value;
            }
        }
    }
}

