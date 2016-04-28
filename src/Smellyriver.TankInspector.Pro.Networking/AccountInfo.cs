using System;
using Smellyriver.TankInspector.Pro.Networking.Remoting.XmlRpc;

namespace Smellyriver.TankInspector.Pro.Networking
{
    public struct AccountInfo
    {
        [XmlRpcMember("is_email_verified")]
        public bool IsEmailVerified;

        [XmlRpcMember("is_premium")]
        public bool IsPremium;

        [XmlRpcMember("create_time")]
        public DateTime CreateTime;

        [XmlRpcMember("country_id")]
        public string CountryId;

        [XmlRpcMember("duration_time")]
        public double DurationTimeSeconds;

        [XmlRpcMember("last_seen_on")]
        public DateTime LastSeenOn;

        [XmlRpcMember("last_country_id")]
        public string LastCountryId;

        [XmlRpcMember("last_login_version")]
        public int LastLoginVersion;
    }
}
