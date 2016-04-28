using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.LivestatProvider
{
    static class TypeCompDescr
    {
        public const uint TankTypeId = 1;

        public static uint GetNationId(string nationKey)
        {
            switch (nationKey)
            {
                case "ussr":
                    return 0;
                case "germany":
                    return 1;
                case "usa":
                    return 2;
                case "china":
                    return 3;
                case "france":
                    return 4;
                case "uk":
                    return 5;
                case "japan":
                    return 6;
                default:
                    return 7;
            }
        }

        public static uint Calculate(IXQueryable tank)
        {
            return TypeCompDescr.Calculate(TypeCompDescr.GetNationId(tank["nation/@key"]), tank.QueryValue<uint>("id"));
        }

        public static uint Calculate(uint typeId, uint nationId, uint tankId)
        {
            return (tankId << 8) + (nationId << 4) + typeId;
        }

        public static uint Calculate(uint nationId, uint tankId)
        {
            return TypeCompDescr.Calculate(TankTypeId, nationId, tankId);
        }
    }
}
