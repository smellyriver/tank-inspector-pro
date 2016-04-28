namespace Smellyriver.TankInspector.Pro.Networking
{
    public enum RegisterResult : int
    {
        successfully_registered = 0,
        email_already_in_use = 1,
        invalid_invitation_code = 2,
        email_name_too_long = 3,
        password_too_long = 4,
        invalid_request = 5,
        cant_create_account = 6,
        invalid_email_name = 7,
    }
}
