namespace Smellyriver.TankInspector.Pro.Networking
{
    public enum CheckResult : int
    {
        everything_ok = 0,
        exists_notification = 1,
        invalid_request = 2,
        invalid_user = 3,
        invalid_session = 4,
        logged_in_elsewhere = 5,
    }
}
