namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class ByteEx
    {
        public static byte Interpolate(byte a, byte b, double offset)
        {
            return (byte)(a + (b - a) * offset);
        }
    }
}
