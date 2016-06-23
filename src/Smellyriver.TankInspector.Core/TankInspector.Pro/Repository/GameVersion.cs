using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;


namespace Smellyriver.TankInspector.Pro.Repository
{
    [DataContract]
    public struct GameVersion : IComparable<GameVersion>, IComparable
    {
        public static readonly DataContractSerializer Serializer;

        static GameVersion()
        {
            Serializer = new DataContractSerializer(typeof(GameVersion));
        }

        private static bool TryParseUInt(string value, out uint result)
        {
            return uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        private static bool TryParseInt(string value, out int result)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        public static GameVersion Parse(string versionString)
        {
            var match = Regex.Match(versionString, @"^\s*v\.(?<major>\d+)\.(?<minor>\d+)\.(?<build>\d+)(\.(?<subbuild>\d+))* (?<ct>Common Test )?(?<sb>Sandbox )?\#(?<revision>\d+)\s*$");
            uint major, minor, build, revision;
            var subbuild = -1;
            bool isCommonTest, isSandbox;
            if (match.Success)
            {
                if (GameVersion.TryParseUInt(match.Groups["major"].Value, out major))
                    if (GameVersion.TryParseUInt(match.Groups["minor"].Value, out minor))
                        if (GameVersion.TryParseUInt(match.Groups["build"].Value, out build))
                        {
                            if (match.Groups["subbuild"] != null)
                            {
                                int subbuildValue;
                                if (GameVersion.TryParseInt(match.Groups["subbuild"].Value, out subbuildValue))
                                    subbuild = subbuildValue;
                            }

                            if (GameVersion.TryParseUInt(match.Groups["revision"].Value, out revision))
                            {
                                if (string.IsNullOrEmpty(match.Groups["ct"].Value))
                                    isCommonTest = false;
                                else
                                    isCommonTest = true;

                                if (string.IsNullOrEmpty(match.Groups["sb"].Value))
                                    isSandbox = false;
                                else
                                    isSandbox = true;

                                return new GameVersion
                                {
                                    Major = major,
                                    Minor = minor,
                                    Build = build,
                                    SubBuild = subbuild,
                                    Revision = revision,
                                    IsCommonTest = isCommonTest,
                                    IsSandbox = isSandbox
                                };
                            }
                        }
            }

            return new GameVersion();
        }

        public string VersionString
        {
            get
            {
                var postfix = this.IsCommonTest
                                  ? " " + Core.Support.Localize("game_client_manager", "common_test")
                                  : this.IsSandbox
                                        ? " " + Core.Support.Localize("game_client_manager", "sandbox")
                                        : null;
                if (this.SubBuild < 0)
                    return string.Format("{0}.{1}.{2}.{3}{4}",
                                         this.Major,
                                         this.Minor,
                                         this.Build,
                                         this.Revision,
                                         postfix);
                else
                    return string.Format("{0}.{1}.{2}.{3}.{4}{5}",
                                         this.Major,
                                         this.Minor,
                                         this.Build,
                                         this.SubBuild,
                                         this.Revision,
                                         postfix);
            }
        }


        public string ShortVersionString
        {
            get
            {
                var postfix = this.IsCommonTest
                                  ? " " + Core.Support.Localize("game_client_manager", "common_test_abbrev")
                                  : this.IsSandbox
                                        ? " " + Core.Support.Localize("game_client_manager", "sandbox_abbrev")
                                        : null;
                return string.Format("{0}.{1}{2}",
                                     this.Minor,
                                     this.Build,
                                     postfix);
            }
        }

        [DataMember]
        public uint Major { get; set; }
        [DataMember]
        public uint Minor { get; set; }
        [DataMember]
        public uint Build { get; set; }
        [DataMember]
        public int SubBuild { get; set; }
        [DataMember]
        public uint Revision { get; set; }
        [DataMember]
        public bool IsCommonTest { get; set; }
        [DataMember]
        public bool IsSandbox { get; set; }


        public override string ToString()
        {
            return this.VersionString;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GameVersion))
                return false;

            var other = (GameVersion)obj;
            return this.Major == other.Major
                && this.Minor == other.Minor
                && this.Build == other.Build
                && this.SubBuild == other.SubBuild
                && this.Revision == other.Revision
                && this.IsCommonTest == other.IsCommonTest
                && this.IsSandbox == other.IsSandbox;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }


        public int CompareTo(GameVersion other)
        {
            var result = this.Major.CompareTo(other.Major);
            if (result != 0) return result;
            result = this.Minor.CompareTo(other.Minor);
            if (result != 0) return result;
            result = this.Build.CompareTo(other.Build);
            if (result != 0) return result;
            if (this.SubBuild >= 0 && other.SubBuild < 0)
                return 1;

            if (this.SubBuild < 0 && other.SubBuild >= 0)
                return -1;

            if (this.SubBuild >= 0 && other.SubBuild >= 0)
            {
                result = this.SubBuild.CompareTo(other.SubBuild);
                if (result != 0) return result;
            }
            return this.Revision.CompareTo(other.Revision);
        }

        public static bool operator ==(GameVersion v1, GameVersion v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(GameVersion v1, GameVersion v2)
        {
            return !v1.Equals(v2);
        }

        public static bool operator >(GameVersion v1, GameVersion v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator <(GameVersion v1, GameVersion v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is GameVersion)
            {
                var other = (GameVersion)obj;
                return this.CompareTo(other);
            }
            else
                return 1;
        }
    }
}
