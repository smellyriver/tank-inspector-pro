using System.ComponentModel;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting
{
    public abstract class AbstractScriptingContext : IScriptingContext
    {
        private readonly ScriptOutputDispatcher _output = new ScriptOutputDispatcher();
        private string _prompt;

        protected AbstractScriptingContext(string defaultPrompt)
        {
            _prompt = defaultPrompt;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Prompt
        {
            get { return _prompt; }
            protected set
            {
                _prompt = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Prompt"));
            }
        }

        protected ScriptOutputDispatcher Output
        {
            get { return _output; }
        }

        public virtual bool VerifyCommand(string command)
        {
            return true;
        }

        public abstract void ExecuteCommand(string command);

        public virtual void AddOutputWriter(IScriptOutputWriter writer)
        {
            _output.Add(writer);
        }

        public virtual void RemoveOutputWriter(IScriptOutputWriter writer)
        {
            _output.Remove(writer);
        }

        public virtual void Clear()
        {
            _output.Clear();
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }


        public virtual void Print(params object[] objects)
        {
            foreach(var o in objects)
            {
                _output.Write(o.ToString());
            }
        }
    }
}