using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Smellyriver.TankInspector.IO.XmlDecoding;
using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    public class ModelVisual : XQueryableWrapper
    {

        private readonly Lazy<List<ModelRenderSet>> _lazyRenderSets;
        public List<ModelRenderSet> RenderSets { get { return _lazyRenderSets.Value; } }

        protected ModelVisual(IXQueryable data)
            : base(data)
        {
            _lazyRenderSets = new Lazy<List<ModelRenderSet>>(this.ReadRenderSets);
        }

        private List<ModelRenderSet> ReadRenderSets()
        {
            return this.QueryMany("renderSet").Select(r => new ModelRenderSet(r, this))
                                              .ToList();
        }

        public static ModelVisual ReadFrom(Stream visualStream)
        {
            using (var reader = new BigworldXmlReader(visualStream))
            {
                var element = XElement.Load(reader);
                return new ModelVisual(element.ToXQueryable());
            }
        }
    }
}
