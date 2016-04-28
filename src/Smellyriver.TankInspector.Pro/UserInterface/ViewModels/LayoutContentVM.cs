using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Modularity;

namespace Smellyriver.TankInspector.Pro.UserInterface.ViewModels
{
    abstract class LayoutContentVM : NotificationObject
    {

        public Guid Guid { get { return this.ContentInfo.Guid; } }
        public string ContentId { get { return this.Guid.ToString(); } }
        public string Title { get { return this.ContentInfo.Title; } }
        public string Description { get { return this.ContentInfo.Description; } }
        public FrameworkElement Content { get { return this.ContentInfo.Content; } }
        public virtual ImageSource IconSource { get { return this.ContentInfo.IconSource; } }
        protected LayoutContentInfo ContentInfo { get; }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                this.RaisePropertyChanged(() => this.IsVisible);
            }
        }

        public LayoutContentVM(LayoutContentInfo contentInfo)
        {
            this.ContentInfo = contentInfo;
            this.IsVisible = true;

            this.ContentInfo.PropertyChanged += ContentInfo_PropertyChanged;
        }

        void ContentInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName);
        }
    }
}
