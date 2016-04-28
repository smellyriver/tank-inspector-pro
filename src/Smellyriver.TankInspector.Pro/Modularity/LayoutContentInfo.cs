using System;
using System.Windows;
using System.Windows.Media;
using Smellyriver.TankInspector.Common;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public abstract class LayoutContentInfo : NotificationObject
    {
        public Guid Guid { get; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                this.RaisePropertyChanged(() => this.Title);
            }
        }

        private FrameworkElement _content;
        public FrameworkElement Content
        {
            get { return _content; }
            set
            {
                _content = value;
                this.RaisePropertyChanged(() => this.Content);
            }
        }

        private ImageSource _iconSource;
        public ImageSource IconSource
        {
            get { return _iconSource; }
            set
            {
                _iconSource = value;
                this.RaisePropertyChanged(() => this.IconSource);
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                this.RaisePropertyChanged(() => this.Description);
            }
        }

        public LayoutContentInfo(Guid guid)
        {
            this.Guid = guid;
        }

        public override bool Equals(object obj)
        {
            var other = obj as LayoutContentInfo;
            if (other == null)
                return false;

            return other.Guid == this.Guid;
        }

        public override int GetHashCode()
        {
            return this.Guid.GetHashCode();
        }
    }
}
