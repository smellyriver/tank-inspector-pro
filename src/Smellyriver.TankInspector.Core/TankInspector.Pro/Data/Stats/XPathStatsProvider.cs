using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;


namespace Smellyriver.TankInspector.Pro.Data.Stats
{
    internal sealed class XPathStatsProvider : IStatsProvider
    {

        private static readonly DataContractSerializer s_serializer;

        static XPathStatsProvider()
        {
            s_serializer = new DataContractSerializer(typeof(XPathStat[]), "Stats", Stat.Xmlns);
        }

        private readonly IEnumerable<XPathStat> _stats;

        public IEnumerable<XPathStat> Stats
        {
            get { return _stats; }
        }

        IEnumerable<IStat> IStatsProvider.Stats
        {
            get { return this.Stats.Cast<IStat>(); }
        }

        private readonly HashSet<XPathStat> _preprocessSet;
        private readonly HashSet<XPathStat> _preprocessedSet;
        private readonly Dictionary<string, XPathStat> _statsLookup;

        public XPathStatsProvider()
        {

            using (var stream = Core.Support.GetStatsXmlStream())
            {
                _stats = (XPathStat[])s_serializer.ReadObject(stream);
            }
            
            _preprocessSet = new HashSet<XPathStat>();
            _preprocessedSet = new HashSet<XPathStat>();
            _statsLookup = new Dictionary<string, XPathStat>();

            foreach (var stat in this.Stats)
            {
                if (Core.Support.EnableXPathStatDebug && _statsLookup.ContainsKey(stat.Key))
                {
                    Core.Support.LogWarning(this,
                                                                string.Format(
                                                                              "a stat with the same key '{0}' is already existed. the latter one will be ignored.",
                                                                              stat.Key));
                    continue;
                }
                _statsLookup.Add(stat.Key, stat);
            }

            foreach (var stat in this.Stats)
            {
                this.PreprocessStat(stat);
            }
        }

        private void PreprocessStat(XPathStat stat)
        {
            if (Core.Support.EnableXPathStatDebug && _preprocessSet.Contains(stat))
            {
                Core.Support.LogError(this, "circular reference detected in stats xml file");
                return;
            }

            if (_preprocessedSet.Contains(stat))
                return;

            _preprocessSet.Add(stat);

            if (Core.Support.EnableXPathStatDebug)
                Core.Support.LogInfo(this,
                                                         string.Format("preprocessing stat '{0}': BaseValueXPath={1}",
                                                                       stat.Key,
                                                                       stat.BaseValueXPath));

            stat.BaseValueXPath = this.HostStatMacro(stat, stat.BaseValueXPath, s => s.BaseValueXPath);

            if (Core.Support.EnableXPathStatDebug)
                Core.Support.LogInfo(this,
                                                         string.Format("{0}.BaseValueXPath expanded to {1}",
                                                                       stat.Key,
                                                                       stat.BaseValueXPath));

            if (Core.Support.EnableXPathStatDebug)
                Core.Support.LogInfo(this,
                                                         string.Format("preprocessing stat '{0}': XPath={1}",
                                                                       stat.Key,
                                                                       stat.XPath));

            stat.XPath = this.HostBaseMacro(stat, stat.XPath);
            stat.XPath = this.HostStatMacro(stat, stat.XPath, s => s.XPath);

            if (Core.Support.EnableXPathStatDebug)
                Core.Support.LogInfo(this,
                                                         string.Format("{0}.XPath expanded to {1}", stat.Key, stat.XPath));

            if (stat.ShowConditionXPath != null)
            {
                if (Core.Support.EnableXPathStatDebug)
                    Core.Support.LogInfo(this,
                                                             string.Format(
                                                                           "preprocessing stat '{0}': ShowConditionXPath={1}",
                                                                           stat.Key,
                                                                           stat.ShowConditionXPath));

                stat.ShowConditionXPath = this.HostValueMacro(stat, stat.ShowConditionXPath);
                stat.ShowConditionXPath = this.HostBaseMacro(stat, stat.ShowConditionXPath);
                stat.ShowConditionXPath = this.HostStatMacro(stat, stat.ShowConditionXPath, s => s.XPath);

                if (Core.Support.EnableXPathStatDebug)
                    Core.Support.LogInfo(this,
                                                             string.Format("{0}.ShowConditionXPath expanded to {1}",
                                                                           stat.Key,
                                                                           stat.ShowConditionXPath));
            }

            if (Core.Support.EnableXPathStatDebug)
                Core.Support.LogInfo(this,
                                                         string.Format("preprocessing stat '{0}': ComparisonXPath={1}",
                                                                       stat.Key,
                                                                       stat.ComparisonXPath));

            stat.ComparisonXPath = this.HostValueMacro(stat, stat.ComparisonXPath);
            stat.ComparisonXPath = this.HostBaseMacro(stat, stat.ComparisonXPath);

            if (Core.Support.EnableXPathStatDebug)
                Core.Support.LogInfo(this,
                                                         string.Format("{0}.ComparisonXPath expanded to {1}",
                                                                       stat.Key,
                                                                       stat.ComparisonXPath));

            _preprocessedSet.Add(stat);
            _preprocessSet.Remove(stat);
        }

        private string HostStatMacro(XPathStat stat, string xpath, Func<XPathStat, string> xpathGetter)
        {
            return Regex.Replace(xpath, @"\#stat\(\s*(\w+)\s*\)", m => this.ExpandStatMacro(m, xpathGetter));
        }

        private string ExpandStatMacro(Match match, Func<XPathStat, string> xpathGetter)
        {

            XPathStat otherStat;
            if (_statsLookup.TryGetValue(match.Groups[1].Value, out otherStat))
            {
                this.PreprocessStat(otherStat);
                return string.Format("({0})", xpathGetter(otherStat));
            }

            return null;
        }

        private string HostBaseMacro(XPathStat stat, string xpath)
        {
            if (string.IsNullOrEmpty(stat.RawBaseValueXPath))
                return xpath;

            return Regex.Replace(xpath, @"\#base", string.Format("({0})", stat.BaseValueXPath));
        }

        private string HostValueMacro(XPathStat stat, string xpath)
        {
            return Regex.Replace(xpath, @"\#value", string.Format("({0})", stat.XPath));
        }
    }
}
