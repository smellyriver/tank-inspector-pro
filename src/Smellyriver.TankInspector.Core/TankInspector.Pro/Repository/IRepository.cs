using Smellyriver.TankInspector.Pro.Data;

namespace Smellyriver.TankInspector.Pro.Repository
{
    public interface IRepository
    {
        string ID { get; }
        string Name { get; }
        string Path { get; }
        string Description { get; }
        GameVersion Version { get; }
        string Language { get; }
        string[] Nations { get; }
        IRepositoryLocalization Localization { get; }

        IXQueryable TankDatabase { get; }
        IXQueryable EquipmentDatabase { get; }
        IXQueryable ConsumableDatabase { get; }
        IXQueryable CrewDatabase { get; }
        IXQueryable CustomizationDatabase { get; }
        IXQueryable TechTreeLayoutDatabase { get; }

        IXQueryable GetTank(string nationKey, string tankKey);
    }
}
