using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Smellyriver.TankInspector.Pro.Repository;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public static class AccessoryExtensions
    {
        public static ImageSource GetIcon(this Accessory @this, IRepository repository)
        {
            var localGameClient = repository as LocalGameClient;
            if (localGameClient != null)
                return localGameClient.PackageImages.GetAccessoryIcon(@this.IconKey);

            return null;
        }
    }
}
