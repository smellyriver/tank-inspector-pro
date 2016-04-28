using System.Windows;
using System.Windows.Controls;
using Smellyriver.TankInspector.Pro.Data.Tank;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.ModelShared
{

    public partial class VersionTag : UserControl
    {

        public TankInstance TankInstance
        {
            get { return (TankInstance)GetValue(TankInstanceProperty); }
            set { SetValue(TankInstanceProperty, value); }
        }

        public static readonly DependencyProperty TankInstanceProperty =
            DependencyProperty.Register("TankInstance", typeof(TankInstance), typeof(VersionTag), new PropertyMetadata(null, VersionTag.OnTankInstanceChanged));

        private static void OnTankInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var versionTag = (VersionTag)d;

            versionTag.ContentContainer.Visibility = versionTag.TankInstance == null
                                                   ? Visibility.Hidden
                                                   : Visibility.Visible;

            if (versionTag.TankInstance == null)
                return;

            var repositoryConfig = RepositoryManager.Instance.GetConfiguration(versionTag.TankInstance.Repository);
            
            versionTag.VersionTagText.Text = versionTag.L("model_shared", "version_tag_format", 
                                                          versionTag.TankInstance.Tank.Name,
                                                          repositoryConfig.Alias);
        }

        public VersionTag()
        {
            InitializeComponent();
        }
    }
}
