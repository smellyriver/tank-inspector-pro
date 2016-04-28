namespace Smellyriver.TankInspector.Pro.Repository
{
    public interface IRepositoryLocalization
    {
        string GetLocalizedNationName(string nation);
        string GetLocalizedTankName(string nation, string key);
        string GetLocalizedClassName(string @class);
    }
}
