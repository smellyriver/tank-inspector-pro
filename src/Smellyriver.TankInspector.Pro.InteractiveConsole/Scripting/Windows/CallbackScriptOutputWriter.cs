using System;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting.Windows
{
    internal class CallbackScriptOutputWriter : IScriptOutputWriter
    {
        private readonly Action<string> _writeCallback;
        private readonly Action _clearCallback;
        private bool _lastWriteEndedWithNewline;
        private bool _isClear;

        public CallbackScriptOutputWriter(Action<string> writeCallback, Action clearCallback)
        {
            _writeCallback = writeCallback;
            _clearCallback = clearCallback;
            _isClear = true;
        }

        public ConsoleColor? ForegroundColor { get; set; }

        public void Clear()
        {
            _isClear = true;
            _clearCallback();
        }

        public void EnsureNewLine()
        {
            if (_isClear) return;

            if (!_lastWriteEndedWithNewline)
            {
                Write(Environment.NewLine);
            }
        }

        public void WriteLine(string text)
        {
            Write(text + Environment.NewLine);
        }

        public void Write(string text)
        {
            _isClear = false;
            _writeCallback(text);
            _lastWriteEndedWithNewline = text.EndsWith(Environment.NewLine);
        }
    }
}
