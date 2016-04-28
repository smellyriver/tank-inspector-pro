using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using Smellyriver.TankInspector.Common.Utilities;
using StiproLocalization = Smellyriver.TankInspector.Pro.Globalization.Localization;

namespace Smellyriver.TankInspector.Pro.Globalization.Wpf
{
    public class LocExtension : MarkupExtension
    {
        public string Catalog { get; set; }
        public string Key { get; }

        public string Default { get; set; }

        public LocExtension(string key)
        {
            this.Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var targetProvider = serviceProvider.GetService<IProvideValueTarget>();
            var target = targetProvider.TargetObject as DependencyObject;

            if ((target == null && string.IsNullOrEmpty(this.Catalog))
                || (target != null && DesignerProperties.GetIsInDesignMode(target)))
                return this.Default ?? string.Format("loc:{0}", this.Key);

            var rootObjectProvider = serviceProvider.GetService<IRootObjectProvider>();

            Assembly assembly = null;
            if (rootObjectProvider.RootObject == null)
                assembly = Assembly.GetEntryAssembly();
            else
            {
                var rootDependencyObject = rootObjectProvider.RootObject as DependencyObject;
                if (rootDependencyObject != null)
                    assembly = Loc.GetAssembly(rootDependencyObject);

                assembly = assembly ?? rootObjectProvider.RootObject.GetType().Assembly;
            }

            var catalogName = this.Catalog;

            if (catalogName == null)
            {
                if (target != null)
                    catalogName = Loc.GetCatalogName(target);
            }

            return StiproLocalization.Instance.Translate(this.Key, catalogName, assembly, null, this.Default);
        }
    }
}
