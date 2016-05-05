using System;
using System.IO;
using System.Text;

namespace Smellyriver.TankInspector.IO.Gettext
{
    unsafe class FastBinaryReader : IDisposable
    {
        private static readonly byte[] s_buffer = new byte[256];
        private readonly Stream _baseStream;

        public Stream BaseStream
        {
            get { return _baseStream; }
        }

        public FastBinaryReader(Stream input)
        {
            _baseStream = input;
        }

        public int ReadInt32()
        {
            BaseStream.Read(s_buffer, 0, 4);
            fixed (byte* numRef = &(s_buffer[0]))
            {
                return *(((int*)numRef));
            }
        }

        public uint ReadUint32()
        {
            BaseStream.Read(s_buffer, 0, 4);
            fixed (byte* numRef = &(s_buffer[0]))
            {
                return *(((uint*)numRef));
            }
        }

        public void Dispose()
        {
            
        }

        public string ReadString(int length)
        {
            if (length == 0)
                return "";

            byte[] buffer = new byte[length];

            BaseStream.Read(buffer, 0, length);

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
