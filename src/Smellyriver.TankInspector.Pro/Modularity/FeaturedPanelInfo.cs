using System;
using System.Collections.Generic;

namespace Smellyriver.TankInspector.Pro.Modularity
{
    public class FeaturedPanelInfo : PanelInfo
    {

        public override Action<DocumentInfo> ActiveDocumentChangedCallback
        {
            get { return base.ActiveDocumentChangedCallback; }
            set { throw new InvalidOperationException("this property cannot be set"); }
        }


        private readonly Type[] _requiredFeatrues;
        public IEnumerable<Type> RequiredFeatures { get { return _requiredFeatrues; } }

        public bool AreRequiredFeaturesSatisified { get; private set; }

        public Action<DocumentInfo, bool> RequiredFeaturesSatisficationChangedCallback { get; }

        public FeaturedPanelInfo(Guid guid, Type[] requiredFeatures, Action<DocumentInfo, bool> requiredFeaturesSatisficationChangedCallback)
            : base(guid)
        {
            _requiredFeatrues = requiredFeatures;
            this.RequiredFeaturesSatisficationChangedCallback = requiredFeaturesSatisficationChangedCallback;
            base.ActiveDocumentChangedCallback = this.OnActiveDocumentChangedCallback;
            this.OnActiveDocumentChangedCallback(null);
        }

        private void OnActiveDocumentChangedCallback(DocumentInfo document)
        {
            var satisifed = true;
            if (document == null)
            {
                satisifed = _requiredFeatrues.Length == 0;
            }
            else
            {
                foreach (var feature in _requiredFeatrues)
                {
                    if (document.GetFeature(feature) == null)
                    {
                        satisifed = false;
                        break;
                    }
                }
            }

            this.AreRequiredFeaturesSatisified = satisifed;
            this.InvokeRequiredFeaturesSatisficationChangedCallback(document);

        }

        private void InvokeRequiredFeaturesSatisficationChangedCallback(DocumentInfo document)
        {
            if (this.RequiredFeaturesSatisficationChangedCallback != null)
                this.RequiredFeaturesSatisficationChangedCallback(document, this.AreRequiredFeaturesSatisified);
        }
    }
}
