namespace Smellyriver.TankInspector.Pro.IO
{
    public class PackageManager
    {
        public static PackageManager Instance { get; private set; }

        static PackageManager()
        {
            PackageManager.Instance = new PackageManager();
        }

        private PackageManager()
        {

        }

        public void Extract(string packagePath,
                            string localPath,
                            string destination,
                            OverwriteStrategy overwriteStrategy = OverwriteStrategy.Ask,
                            bool useRelativePath = true)
        {
            var action = new ExtractPackageContentAction(packagePath, localPath, destination, overwriteStrategy, useRelativePath);
            action.BeginAsync();
        }


        public void Extract(string[] fileList, string destination, OverwriteStrategy overwriteStrategy = OverwriteStrategy.Ask)
        {
            var action = new ExtractPackageContentAction(fileList, destination, overwriteStrategy);
            action.BeginAsync();
        }
    }
}
