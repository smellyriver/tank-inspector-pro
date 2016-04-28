using System;
using System.Globalization;
using SharpDX;
using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.Graphics
{
    public static class IXQueryableExtensions
    {
        public static Vector3 QueryVector3(this IXQueryable queryable, string xpath)
        {
            var queryResult = queryable[xpath];
            if (queryResult == null)
                return new Vector3();


            var values = queryResult.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            return new Vector3
            {
                X = float.Parse(values[0], CultureInfo.InvariantCulture),
                Y = float.Parse(values[1], CultureInfo.InvariantCulture),
                Z = float.Parse(values[2], CultureInfo.InvariantCulture)
            };
        }

        public static Vector4 QueryVector4(this IXQueryable queryable, string xpath)
        {
            var queryResult = queryable[xpath];
            if (queryResult == null)
                return new Vector4();

            var values = queryResult.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            return new Vector4
            {
                X = float.Parse(values[0], CultureInfo.InvariantCulture),
                Y = float.Parse(values[1], CultureInfo.InvariantCulture),
                Z = float.Parse(values[2], CultureInfo.InvariantCulture),
                W = float.Parse(values[3], CultureInfo.InvariantCulture)
            };
        }

        public static Color QueryColor(this IXQueryable queryable, string xpath)
        {
            var queryResult = queryable[xpath];
            if (queryResult == null)
                return new Color();

            var values = queryResult.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            return new Color
            {
                R = byte.Parse(values[0], CultureInfo.InvariantCulture),
                G = byte.Parse(values[1], CultureInfo.InvariantCulture),
                B = byte.Parse(values[2], CultureInfo.InvariantCulture),
                A = byte.Parse(values[3], CultureInfo.InvariantCulture)
            };
        }
    }
}
