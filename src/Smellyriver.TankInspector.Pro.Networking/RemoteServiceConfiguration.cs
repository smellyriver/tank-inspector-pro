namespace Smellyriver.TankInspector.Pro.Networking
{
    static class RemoteServiceConfiguration
    {

#if !DEBUG || SINA
        public const string ServerURL = "http://tank.sinaapp.com/RPC";
        public const int PingIntervalMilliseconds = 240000;
#else
        public const string ServerURL = "http://edward-i7:8000/RPC";
        public const int PingIntervalMilliseconds = 10000;
#endif

    }
}
