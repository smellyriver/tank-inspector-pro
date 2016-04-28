using System.Windows.Input;

namespace Smellyriver.TankInspector.Pro.ModelInspector
{
    partial class ModelDocumentVM
    {
        protected override void InitializeCommands()
        {
            base.InitializeCommands();

            this.CommandBindings.Add(new CommandBinding(Commands.SwitchToUndamagedModel,
                                                        this.SwitchToUndamagedModel,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.UseUndamagedModel)));
            this.CommandBindings.Add(new CommandBinding(Commands.SwitchToDestroyedModel,
                                                        this.SwitchToDestroyedModel,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.UseDestroyedModel)));
            this.CommandBindings.Add(new CommandBinding(Commands.SwitchToExplodedModel,
                                                        this.SwitchToExplodedModel,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.UseExplodedModel)));
            this.CommandBindings.Add(new CommandBinding(Commands.SwitchToNormalTextureMode,
                                                        this.SwitchToNormalTextureMode,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.UseNormalTexture)));
            this.CommandBindings.Add(new CommandBinding(Commands.SwitchToGridTextureMode,
                                                        this.SwitchToGridTextureMode,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.UseGridTexture)));
            this.CommandBindings.Add(new CommandBinding(Commands.SwitchToOfficialTextureSource,
                                                        this.SwitchToOfficialTextureSource,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.UseOfficialTexture)));
            this.CommandBindings.Add(new CommandBinding(Commands.SwitchToModTextureSource,
                                                        this.SwitchToModTextureSource,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.UseModTexture)));
            this.CommandBindings.Add(new CommandBinding(Commands.ToggleCamouflage,
                                                        this.ToggleCamouflage,
                                                        this.CreateMenuItemCheckStateUpdator(() => this.ShowCamouflage)));

        }

        private void SwitchToUndamagedModel(object sender, ExecutedRoutedEventArgs e)
        {
            this.UseUndamagedModel = true;
        }

        private void SwitchToDestroyedModel(object sender, ExecutedRoutedEventArgs e)
        {
            this.UseDestroyedModel = true;
        }

        private void SwitchToExplodedModel(object sender, ExecutedRoutedEventArgs e)
        {
            this.UseExplodedModel = true;
        }

        private void SwitchToNormalTextureMode(object sender, ExecutedRoutedEventArgs e)
        {
            this.UseNormalTexture = true;
        }

        private void SwitchToGridTextureMode(object sender, ExecutedRoutedEventArgs e)
        {
            this.UseGridTexture = true;
        }

        private void SwitchToOfficialTextureSource(object sender, ExecutedRoutedEventArgs e)
        {
            this.UseOfficialTexture = true; 
        }

        private void SwitchToModTextureSource(object sender, ExecutedRoutedEventArgs e)
        {
            this.UseModTexture = true;
        }

        private void ToggleCamouflage(object sender, ExecutedRoutedEventArgs e)
        {
            this.ShowCamouflage = !this.ShowCamouflage;
        }

    }
}
