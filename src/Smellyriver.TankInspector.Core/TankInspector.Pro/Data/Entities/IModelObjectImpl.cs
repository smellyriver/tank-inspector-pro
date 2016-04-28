using System;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    internal static class IModelObjectImpl
    {
        public static string GetModelPath<T>(T modelObject, ModelType type)
            where T : TankObject, IModelObject
        {
            switch (type)
            {
                case ModelType.Undamaged:
                    return modelObject["models/undamaged"];
                case ModelType.Collision:
                    return modelObject["hitTester/collisionModel"];
                case ModelType.Destroyed:
                    return modelObject["models/destroyed"];
                case ModelType.Exploded:
                    return modelObject["models/exploded"];
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
