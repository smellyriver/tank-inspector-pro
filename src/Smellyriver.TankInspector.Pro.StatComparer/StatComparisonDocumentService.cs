using System;
using System.Threading.Tasks;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.Modularity.Features;
using Smellyriver.TankInspector.Pro.Modularity.Modules;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;

namespace Smellyriver.TankInspector.Pro.StatComparer
{
    class StatComparisonDocumentService : StatComparisonDocumentServiceBase
    {
        public static readonly StatComparisonDocumentService Instance = new StatComparisonDocumentService();

        private readonly Guid _guid = Guid.Parse("BB35870C-0C9D-42A8-B800-6CE184997DA4");
        public override Guid Guid
        {
            get { return _guid; }
        }

        private StatComparisonDocumentService()
        {

        }

        public Task<string> PromptForTitle()
        {
            var settings = new InputDialogSettings()
                {
                    DefaultText = this.L("stat_comparer", "new_comparison_default_title"),
                };

            return DialogManager.Instance.ShowInputDialogAsync(this.L("stat_comparer", "new_comparison_title_prompt_dialog_title"),
                                                               this.L("stat_comparer", "new_comparison_title_prompt_dialog_message"), 
                                                               settings);
        }

        public override ICreateDocumentTask CreateCreateDocumentTask(Uri uri, Guid guid, string persistentInfo)
        {
            return CreateDocumentTask.FromFactory(() =>
                {
                    var view = new StatComparisonDocumentView();
                    var vm = new StatComparisonDocumentVM(this, view.CommandBindings, persistentInfo);
                    view.ViewModel = vm;

                    var docInfo = new DocumentInfo(guid: guid,
                                                   repositoryId: null,
                                                   uri: uri,
                                                   title: this.L("stat_comparer", "new_comparison_default_title"),
                                                   content: view,
                                                   features: new IFeature[] { vm },
                                                   persistentInfoProvider: vm.PersistentInfo)
                    {
                        IconSource = StatComparerModule.CompareIcon
                    };
                    vm.DocumentInfo = docInfo;

                    return docInfo;
                });
        }
    }
}
