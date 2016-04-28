using System;
using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting
{
    public class ScriptOutputDispatcher : IScriptOutputWriter
    {
        private readonly List<IScriptOutputWriter> _writers = new List<IScriptOutputWriter>();
        private ConsoleColor? _foregroundColor;

        public void Add(IScriptOutputWriter writer)
        {
            writer.ForegroundColor = _foregroundColor;
            _writers.Add(writer);
        }

        public void Remove(IScriptOutputWriter writer)
        {
            _writers.Remove(writer);
        }

        public ConsoleColor? ForegroundColor
        {
            get { return _foregroundColor; }
            set 
            {
                _foregroundColor = value;
                foreach (var writer in _writers) writer.ForegroundColor = value;
            }
        }

        public IDisposable ChangeForegroundColor(ConsoleColor color)
        {
            var currentColor = ForegroundColor;
            ForegroundColor = color;
            return new CallbackDisposable(() => ForegroundColor = currentColor);
        }

        public void Clear()
        {
            foreach (var writer in _writers) writer.Clear();
        }

        public void EnsureNewLine()
        {
            foreach (var writer in _writers) writer.EnsureNewLine();
        }

        public void Write(string format, params object[] values)
        {
            Write(string.Format(format, values));
        }

        public void Write(string text)
        {
            foreach (var writer in _writers) writer.Write(text);
        }

        public void WriteLine(string format, params object[] values)
        {
            WriteLine(string.Format(format, values));
        }

        public void WriteLine(string text)
        {
            foreach (var writer in _writers) writer.WriteLine(text);
        }
    }
}