using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Smellyriver.TankInspector.Pro.TankModuleTree
{
    class TankModuleTreeDocumentVM : NotificationObject, ITankConfigurable
    {
        private static Dictionary<Type, int> s_moduleTypeRowMap
            = new Dictionary<Type, int>
            {
                { typeof(Gun),      0},
                { typeof(Turret),   1},
                { typeof(Radio),    2},
                { typeof(Engine),   3},
                { typeof(Chassis),  4},
            };

        public TankInstance TankInstance { get; private set; }

        public IEnumerable<ModuleVM> Modules { get; private set; }
        public IEnumerable<UnlockedTankVM> UnlockedTanks { get; private set; }

        public IEnumerable<IModuleUnlockTargetVM> UnlockTargets { get; private set; }

        public int Columns { get; private set; }
        public int Rows { get { return s_moduleTypeRowMap.Count; } }

        IRepository ITankConfigurable.Repository
        {
            get { return this.TankInstance.Repository; }
        }

        TankConfiguration ITankConfigurable.TankConfiguration
        {
            get { return this.TankInstance.TankConfiguration; }
        }

        private event EventHandler _tankConfigurationChanged;
        event EventHandler ITankConfigurable.TankConfigurationChanged
        {
            add { _tankConfigurationChanged += value; }
            remove { _tankConfigurationChanged -= value; }
        }

        public TankModuleTreeDocumentVM(TankModuleTreeDocumentService service,
                                        CommandBindingCollection commandBindings,
                                        TankInstance tankInstance,
                                        string persistentInfo)
        {
            this.TankInstance = tankInstance;

            Task.Factory.StartNew(this.AnalyseUnlocks);

        }

        private void AnalyseUnlocks()
        {
            var modules = ((IEnumerable<Module>)this.TankInstance.Tank.Turrets)
                          .Union(this.TankInstance.Tank.Guns)
                          .Union(this.TankInstance.Tank.Engines)
                          .Union(this.TankInstance.Tank.Radios)
                          .Union(this.TankInstance.Tank.Chassis);

            var moduleVms = modules.ToDictionary(m => m, m => new ModuleVM(m));
            var unlockedTankVms = new List<UnlockedTankVM>();
            foreach (var module in modules)
            {
                var unlockVms = new List<UnlockInfoVM>();
                var unlocks = this.TankInstance.Repository.GetUnlocks(this.TankInstance.Tank, module);
                foreach (var unlockInfo in unlocks)
                {
                    var unlockedModule = unlockInfo.Target as Module;
                    if (unlockedModule != null)
                    {
                        ModuleVM unlockedModuleVm;
                        if (moduleVms.TryGetValue(unlockedModule, out unlockedModuleVm))
                            unlockVms.Add(new UnlockInfoVM(unlockedModuleVm, unlockInfo.Experience));
                    }
                    else
                    {
                        var unlockedTank = unlockInfo.Target as Tank;
                        if (unlockedTank != null)
                        {
                            var tankVm = new UnlockedTankVM(unlockedTank, moduleVms[module]);
                            unlockVms.Add(new UnlockInfoVM(tankVm, unlockInfo.Experience));
                            unlockedTankVms.Add(tankVm);
                        }
                    }
                }

                moduleVms[module].Unlocks = unlockVms;
            }

            this.Modules = moduleVms.Values.ToArray();
            this.UnlockedTanks = unlockedTankVms;

            this.UnlockTargets = ((IEnumerable<IModuleUnlockTargetVM>)this.Modules).Union(this.UnlockedTanks);

            this.Columns = this.LayoutModules() + 1;

            foreach (var unlockedTankVm in unlockedTankVms)
            {
                unlockedTankVm.Row = unlockedTankVm.UnlockedBy.Row;
                unlockedTankVm.Column = this.Columns - 1;
            }
        }

        private int LayoutModuleClass(ModuleVM rootModule)
        {
            ++_rowPointer;
            return this.LayoutModuleRecursive(rootModule, _rowPointer, 0);
        }

        private int LayoutModuleRecursive(ModuleVM module, int row, int column)
        {
            this.SetRow(module, row);
            this.SetColumn(module, column);


            foreach(var unlock in module.Unlocks)
            {
                if(unlock.Target is UnlockedTankVM)
                {
                    this.SetRow(unlock.Target, row);
                    _isRowTerminated[row] = true;
                }
            }
        }


    }
}
