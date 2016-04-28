using System;
using System.IO;
using System.Text;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting
{
    public class ScriptOutputWriterStream : Stream
    {
        private readonly IScriptOutputWriter _writer;
        private StringBuilder _buffer;

        public ScriptOutputWriterStream(IScriptOutputWriter writer)
        {
            _writer = writer;
            _buffer = new StringBuilder();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            _writer.Write(_buffer.ToString());
            _buffer = new StringBuilder();
        }

        public override long Length
        {
            get { return _buffer.Length; }
        }

        public override long Position { get; set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _buffer.Append(Encoding.Unicode.GetString(buffer, offset, count));
            Flush();
        }
    }
}