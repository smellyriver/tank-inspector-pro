using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.StatsInspector
{
    class TemplateManager
    {
        public static TemplateManager Instance { get; private set; }

        static TemplateManager()
        {
            TemplateManager.Instance = new TemplateManager();
        }

        private static readonly string s_templatePath = Path.Combine(ApplicationPath.GetModuleDirectory(Assembly.GetCallingAssembly()), "Templates");
        private const string c_defaultTemplateFile = "Default.xaml";
        private static readonly string s_defaultTemplateFileName = TemplateManager.NormalizeTemplateFilename(Path.Combine(s_templatePath, c_defaultTemplateFile));



        private static string NormalizeTemplateFilename(string filename)
        {
            return Path.GetFullPath(filename).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).ToLower();
        }


        private readonly Dictionary<string, StatTemplate> _templates;
        public IEnumerable<StatTemplate> Templates { get { return _templates.Values; } }


        private TemplateManager()
        {
            _templates = Directory.GetFiles(s_templatePath)
                                  .OrderBy(f => Path.GetFileName(f))
                                  .ToDictionary(f => TemplateManager.NormalizeTemplateFilename(f), f => StatTemplate.Load(f));
            _templates.RemoveWhere(i => i.Value == null);

            if (!_templates.ContainsKey(s_defaultTemplateFileName))
            {
                DialogManager.Instance.ShowMessageAsync(
                    this.L("stat_inspector", "missing_default_template_message_title"),
                    this.L("stat_inspector", "missing_default_template_message"));
            }
        }

        public StatTemplate GetTemplate(string templateFilename)
        {
            StatTemplate template;
            if (!_templates.TryGetValue(TemplateManager.NormalizeTemplateFilename(templateFilename), out template))
            {
                this.LogWarning("stats template file missing: {0}", templateFilename);
                templateFilename = s_defaultTemplateFileName;

                if (!_templates.TryGetValue(s_defaultTemplateFileName, out template))
                {
                    return null;
                }

                StatsInspectorSettings.Default.LastTemplateFilename = templateFilename;
                StatsInspectorSettings.Default.Save();
            }

            return template;
        }
    }
}
