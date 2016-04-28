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

        public static GameVersion Parse(string versionString)
        {
            var match = Regex.Match(versionString, @"^\s*v\.(?<major>\d+)\.(?<minor>\d+)\.(?<build>\d+)(\.(?<subbuild>\d+))* (?<ct>Common Test )?\#(?<revision>\d+)\s*$");
            uint major, minor, build, revision;
            uint? subbuild = null;
            bool isCommonTest;
            if (match.Success)
            {
                if (GameVersion.TryParseUInt(match.Groups["major"].Value, out major))
                    if (GameVersion.TryParseUInt(match.Groups["minor"].Value, out minor))
                        if (GameVersion.TryParseUInt(match.Groups["build"].Value, out build))
                        {
                            if (match.Groups["subbuild"] != null)
                            {
                                uint subbuildValue;
                                if (GameVersion.TryParseUInt(match.Groups["subbuild"].Value, out subbuildValue))
                                    subbuild = subbuildValue;
                            }

                            if (GameVersion.TryParseUInt(match.Groups["revision"].Value, out revision))
                            {
                                if (string.IsNullOrEmpty(match.Groups["ct"].Value))
                                    isCommonTest = false;
                                else
                                    isCommonTest = true;


                                return new GameVersion
                                {
                                    Major = major,
                                    Minor = minor,
                                    Build = build,
                                    SubBuild = subbuild,
                                    Revision = revision,
                                    IsCommonTest = isCommonTest
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
                if (this.SubBuild == null)
                    return string.Format("{0}.{1}.{2}.{3}{4}",
                                         this.Major,
                                         this.Minor,
                                         this.Build,
                                         this.Revision,
                                         this.IsCommonTest ? " " + Core.Support.Localize("game_client_manager", "common_test") : null);
                else
                    return string.Format("{0}.{1}.{2}.{3}.{4}{5}",
                                         this.Major,
                                         this.Minor,
                                         this.Build,
                                         this.SubBuild,
                                         this.Revision,
                                         this.IsCommonTest ? " " + Core.Support.Localize("game_client_manager", "common_test") : null);

            }
        }

        public string ShortVersionString
        {
            get
            {
                return string.Format("{0}.{1}{2}",
                                     this.Minor,
                                     this.Build,
                                     this.IsCommonTest ? " " + Core.Support.Localize("game_client_manager", "common_test_abbrev") : null);
            }
        }

        [DataMember]
        public uint Major { get; set; }
        [DataMember]
        public uint Minor { get; set; }
        [DataMember]
        public uint Build { get; set; }
        [DataMember]
        public uint? SubBuild { get; set; }
        [DataMember]
        public uint Revision { get; set; }
        [DataMember]
        public bool IsCommonTest { get; set; }

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
                && this.IsCommonTest == other.IsCommonTest;
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
            if (this.SubBuild != null && other.SubBuild == null)
                return 1;

            if (this.SubBuild == null && other.SubBuild != null)
                return -1;

            if (this.SubBuild != null && other.SubBuild != null)
            {
                result = this.SubBuild.Value.CompareTo(other.SubBuild.Value);
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
