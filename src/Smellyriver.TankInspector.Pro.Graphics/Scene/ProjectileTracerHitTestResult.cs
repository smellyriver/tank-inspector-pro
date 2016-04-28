using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Pro.Graphics.Scene
{

    struct ProjectileTracerHit
    {
        public float Distance;
        public ProjectileTracer Tracer;
    }


    class ProjectileTracerHitTestResult
    {
        public List<ProjectileTracerHit> ProjectileTracerHits = new List<ProjectileTracerHit>();

        public ProjectileTracerHit? ClosesetHit
        {
            get
            {
                 return ProjectileTracerHits.Aggregate((a, b) => a.Distance > b.Distance ? b : a);
            }
        }
    }
}
