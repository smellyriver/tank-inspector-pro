using System;
using System.Windows.Input;
using Smellyriver.TankInspector.Common.Wpf.Mvvm.ViewModels.Menu;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    partial class ModelDocumentVMBase
    {

        protected virtual void InitializeCommands()
        {
            this.CommandBindings.Add(new CommandBinding(Commands.Snapshot, this.TakeSnapshot, this.CanTakeSnapshot));
            this.CommandBindings.Add(new CommandBinding(Commands.ToggleFPSDisplay,
                                                        this.ToggleFPSDisplay,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.ShowFps)));
            this.CommandBindings.Add(new CommandBinding(Commands.ToggleTriangleCountDisplay, 
                                                        this.ToggleTriangleCountDisplay,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.ShowTriangleCount)));
            this.CommandBindings.Add(new CommandBinding(Commands.ToggleShowGun,
                                                        this.ToggleShowGun,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.ShowGun)));
            this.CommandBindings.Add(new CommandBinding(Commands.ToggleShowTurret,
                                                        this.ToggleShowTurret,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.ShowTurret)));
            this.CommandBindings.Add(new CommandBinding(Commands.ToggleShowHull,
                                                        this.ToggleShowHull,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.ShowHull)));
            this.CommandBindings.Add(new CommandBinding(Commands.ToggleShowChassis,
                                                        this.ToggleShowChassis,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.ShowChassis)));
            this.CommandBindings.Add(new CommandBinding(Commands.SwitchToSolidMode,
                                                        this.SwitchToSolidMode,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.UseSolidMode)));
            this.CommandBindings.Add(new CommandBinding(Commands.SwitchToWireframeMode,
                                                        this.SwitchToWireframeMode,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.UseWireframeMode)));
        }

        protected CanExecuteRoutedEventHandler CreateMenuItemCheckStateUpdator(Func<bool> checkStateGetter)
        {
            return (sender, e) =>
                {
                    e.CanExecute = true;

                    var menuItemVm = e.Parameter as MenuItemVM;
                    if (menuItemVm == null)
                        return;
                    menuItemVm.IsChecked = checkStateGetter();
                };
        }

        private void SwitchToWireframeMode(object sender, ExecutedRoutedEventArgs e)
        {
            this.UseWireframeMode = true;
        }

        private void SwitchToSolidMode(object sender, ExecutedRoutedEventArgs e)
        {
            this.UseWireframeMode = false;
        }

        private void ToggleShowChassis(object sender, ExecutedRoutedEventArgs e)
        {
            this.ShowChassis = !this.ShowChassis;
        }

        private void ToggleShowHull(object sender, ExecutedRoutedEventArgs e)
        {
            this.ShowHull = !this.ShowHull;
        }

        private void ToggleShowTurret(object sender, ExecutedRoutedEventArgs e)
        {
            this.ShowTurret = !this.ShowTurret;
        }

        private void ToggleShowGun(object sender, ExecutedRoutedEventArgs e)
        {
            this.ShowGun = !this.ShowGun;
        }

        private void ToggleTriangleCountDisplay(object sender, ExecutedRoutedEventArgs e)
        {
            this.ShowTriangleCount = !this.ShowTriangleCount;
        }

        private void ToggleFPSDisplay(object sender, ExecutedRoutedEventArgs e)
        {
            this.ShowFps = !this.ShowFps;
        }

        protected virtual void CanTakeSnapshot(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        protected virtual void TakeSnapshot(object sender, ExecutedRoutedEventArgs e)
        {
            //todo
        }
    }
}
