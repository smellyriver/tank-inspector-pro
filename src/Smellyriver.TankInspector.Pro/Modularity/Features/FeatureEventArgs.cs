using System;

namespace Smellyriver.TankInspector.Pro.Modularity.Features
{
    public class FeatureEventArgs : EventArgs
    {
        private readonly IFeature _feature;
        public IFeature Feature
        {
            get { return _feature; }
        }

        public FeatureEventArgs(IFeature feature)
        {
            _feature = feature;
        }
    }
}
