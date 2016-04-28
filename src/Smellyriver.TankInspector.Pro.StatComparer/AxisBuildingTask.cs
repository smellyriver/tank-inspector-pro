using System;
using System.Windows.Input;
using Smellyriver.TankInspector.Common.Wpf.Input;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    class AxisBuildingTask
    {
        public string Text { get; private set; }
        public ICommand Command { get; private set; }

        public AxisBuildingTask(string text, Action exectue)
        {
            this.Text = text;
            this.Command = new RelayCommand(exectue);
        }
    }
}
