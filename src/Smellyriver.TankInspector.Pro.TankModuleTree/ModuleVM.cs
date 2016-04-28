using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.TankModuleShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.TankModuleTree
{
    class ModuleVM : NotificationObject, IModuleUnlockTargetVM
    {
        public Module Model { get; private set; }
        public string Name { get { return this.Model.Name; } }
        public double Price { get { return this.Model.Price; } }
        public double UnlockExperience { get { return this.Model.UnlockExperience; } }

        private bool _isEquipped;
        public bool IsEquipped
        {
            get { return _isEquipped; }
            set
            {
                _isEquipped = value;
                this.RaisePropertyChanged(() => this.IsEquipped);
            }
        }

        public int Row { get; set; }
        public int Column { get; set; }

        public ImageSource Icon { get; private set; }

        public IEnumerable<UnlockInfoVM> Unlocks { get; internal set; }

        public ModuleVM(Module module)
        {
            this.Model = module;
            this.Icon = ModuleIcon.GetModuleIcon(module.GetType());
            this.Row = ModuleUnlockTargetVM.UndefinedRowOrColumn;
            this.Column = ModuleUnlockTargetVM.UndefinedRowOrColumn;
        }
    }
}
