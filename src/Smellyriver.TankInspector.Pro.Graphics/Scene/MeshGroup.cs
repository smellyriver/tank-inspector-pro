using System.Collections.Generic;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Graphics.Frameworks;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{

    public class MeshGroup
    {
        public ArmorGroup ArmorGroup { get; set; }

        public IList<Vertex> RawVertices { get; set; }

        public IList<int> RawIndices { get; set; }

        public int StartIndex { get; set; }

        public int PrimitiveCount { get; set; }

        public IEnumerable<Triangle> Triangles
        {
            get
            {
                for (int i = 0; i != PrimitiveCount; ++i)
                {
                    int index = i * 3 + StartIndex;

                    yield return new Triangle()
                    {
                        v1 = DXUtils.Convert(RawVertices[RawIndices[index]].Position),
                        v2 = DXUtils.Convert(RawVertices[RawIndices[index + 1]].Position),
                        v3 = DXUtils.Convert(RawVertices[RawIndices[index + 2]].Position),
                    };
                }
                yield break;
            }
        }
    }

}
