using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Gameplay;
using Smellyriver.TankInspector.Pro.IO;
using Smellyriver.TankInspector.Pro.Modularity;
using Smellyriver.TankInspector.Pro.PatchnoteGenerator.Report;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface.ViewModels;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.PatchnoteGenerator
{
    class PatchnoteGeneratorDocumenVM : FlowDocumentVMBase
    {

        private static readonly string s_documentStyleFile = Path.Combine(ApplicationPath.GetModuleDirectory(Assembly.GetCallingAssembly()), "DocumentStyle.xaml");

        public PatchnoteGeneratorDocumentPersistentInfo PersistentInfo { get; }

        private bool _isGenerating;
        public bool IsGenerating
        {
            get { return _isGenerating; }
            private set
            {
                _isGenerating = value;
                this.RaisePropertyChanged(() => this.IsGenerating);
            }
        }

        private double _generationProgress;
        public double GenerationProgress
        {
            get { return _generationProgress; }
            private set
            {
                _generationProgress = value;
                this.RaisePropertyChanged(() => this.GenerationProgress);
            }
        }

        private bool _isProgressIndeterminate;
        public bool IsProgressIndeterminate
        {
            get { return _isProgressIndeterminate; }
            private set
            {
                _isProgressIndeterminate = value;
                this.RaisePropertyChanged(() => this.IsProgressIndeterminate);
            }
        }

        private string _generationStatus;
        public string GenerationStatus
        {
            get { return _generationStatus; }
            set
            {
                _generationStatus = value;
                this.RaisePropertyChanged(() => this.GenerationStatus);
            }
        }


        private FlowDocument _document;
        public override FlowDocument Document
        {
            get { return _document; }
            protected set
            {
                _document = value;
                this.ApplyDocumentStyle();
                this.RaisePropertyChanged(() => this.Document);
            }
        }

        private ResourceDictionary _documentStyle;
        public ResourceDictionary DocumentStyle
        {
            get { return _documentStyle; }
            set
            {
                var oldStyle = value;
                _documentStyle = value;
                this.ApplyDocumentStyle(oldStyle);
                this.RaisePropertyChanged(() => this.DocumentStyle);
            }
        }

        protected override string ExportFileName
        {
            get
            {
                return this.PersistentInfo.SaveAsPath
                        ?? this.L("patchnote_generator", "default_patchnote_file_name", this.TargetRepository.Version)
                               .Replace(Path.GetInvalidFileNameChars(), '-');
            }
            set
            {
                this.PersistentInfo.SaveAsPath = value;
            }
        }

        private Task _generationTask;

        private int _processedTankTaskCount;
        private int _tankTaskCount;

        private List<PatchnoteReportItem> _addedTanks;
        private List<PatchnoteReportItem> _removedTanks;
        private List<PatchnoteReportItem> _statChanges;
        private List<PatchnoteReportItem> _potentialCollisionChanges;
        private bool _checkPotentialCollisionChanges;

        public IRepository TargetRepository { get; }
        public IRepository ReferenceRepository { get; }

        private LocalGameClient TargetClient { get { return this.TargetRepository as LocalGameClient; } }
        private LocalGameClient ReferenceClient { get { return this.ReferenceRepository as LocalGameClient; } }

        public PatchnoteGeneratorDocumenVM(CommandBindingCollection commandBindings,
                                           IRepository target,
                                           IRepository reference,
                                           string persistentInfo)
            : base(commandBindings)
        {
            this.PersistentInfo = DocumentPersistentInfoProviderBase.Load(persistentInfo,
                                                                          () => new PatchnoteGeneratorDocumentPersistentInfo());

            this.TargetRepository = target;
            this.ReferenceRepository = reference;

            using (var documentStyleFile = File.OpenRead(s_documentStyleFile))
                this.DocumentStyle = (ResourceDictionary)XamlReader.Load(documentStyleFile);

            if (!this.TryLoadPersistentDocument())
            {
                this.GeneratePatchnote();
            }
        }

        private bool TryLoadPersistentDocument()
        {
            if (string.IsNullOrEmpty(this.PersistentInfo.GeneratedDocument))
                return false;

            try
            {
                using (var documentStream = this.PersistentInfo.GeneratedDocument.ToStream())
                {
                    this.Document = XamlReader.Load(documentStream) as FlowDocument;
                    if (this.Document == null)
                    {
                        this.LogError("failed to load persistent patchnote document, the document is empty");
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                this.LogError("failed to load persistent patchnote document: {0}", ex.Message);
                return false;
            }
        }


        private void ApplyDocumentStyle(ResourceDictionary oldStyle = null)
        {
            if (this.Document == null)
                return;

            if (oldStyle != null)
                this.Document.Resources.MergedDictionaries.Remove(oldStyle);

            this.Document.Resources.MergedDictionaries.Add(this.DocumentStyle);
        }


        private void GeneratePatchnote()
        {
            this.IsGenerating = true;
            this.GenerationProgress = 0;
            this.IsProgressIndeterminate = true;
            this.GenerationStatus = this.L("patchnote_generator", "generate_status_preparing");// "Preparing...";

            //this.BeginGeneratePatchnote();
            _generationTask = Task.Factory.StartNew(this.BeginGeneratePatchnote);
            _generationTask.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        this.LogError("failed to generate patchnote: {0}", t.Exception);

                        this.GenerationStatus = this.L("patchnote_generator", "generate_status_failed");// "Generation failed";
                        this.IsProgressIndeterminate = false;
                    }
                });
        }

        private void PersistDocument()
        {
            try
            {
                this.PersistentInfo.GeneratedDocument = XamlWriter.Save(this.Document);
            }
            catch (Exception ex)
            {
                this.LogError("failed to persist patchnote document: {0}", ex.Message);
            }
        }

        private void BeginGeneratePatchnote()
        {
            _addedTanks = new List<PatchnoteReportItem>();
            _removedTanks = new List<PatchnoteReportItem>();
            _statChanges = new List<PatchnoteReportItem>();
            _potentialCollisionChanges = new List<PatchnoteReportItem>();
            _checkPotentialCollisionChanges = this.TargetClient != null && this.ReferenceClient != null;

            var oldTanks = this.ReferenceRepository.TankDatabase.QueryMany("tank");
            var newTanks = this.TargetRepository.TankDatabase.QueryMany("tank");

            var diffResult = oldTanks.Diff(newTanks, TankHelper.KeyEqualityComparer);

            _tankTaskCount = diffResult.Added.Length + diffResult.Removed.Length + diffResult.Shared.Length;
            _processedTankTaskCount = 0;

            this.IsProgressIndeterminate = false;
            this.GenerationStatus = this.L("patchnote_generator", "generate_status_analysing");// "Analysing...";

            this.HandleAddedTanks(diffResult.Added);
            this.HandleRemovedTanks(diffResult.Removed);
            this.HandleSharedTanks(diffResult.Shared);

            this.GenerationStatus = this.L("patchnote_generator", "generate_status_creating_document");// "Creating document...";

            this.InvokeGeneratePatchnoteDocument();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.PersistDocument();
                this.GenerationStatus = this.L("patchnote_generator", "generate_status_completed"); // "Completed";
                this.GenerationProgress = 1.0;
                this.IsGenerating = false;
            }));
        }

        private IEnumerable<Block> CreateTitledList(string title, IEnumerable<PatchnoteReportItem> items)
        {
            if (items.Any())
            {
                var titleParagraph = new Paragraph(new Run(title));
                titleParagraph.SetResourceReference(FrameworkContentElement.StyleProperty, "Title2");
                yield return titleParagraph;
                var list = new List();
                foreach (var item in items)
                {
                    var listItem = new ListItem();
                    listItem.Blocks.AddRange(item.CreateBlocks());
                    list.ListItems.Add(listItem);
                }

                yield return list;
            }

            yield break;
        }

        private IEnumerable<Block> CreateTitledParagraphs(string title,
                                                          IEnumerable<PatchnoteReportItem> items,
                                                          string comment = null,
                                                          bool titlizeFirstParagraph = true)
        {
            if (items.Any())
            {
                var titleParagraph = new Paragraph(new Run(title));
                titleParagraph.SetResourceReference(FrameworkContentElement.StyleProperty, "Title2");
                yield return titleParagraph;

                if (comment != null)
                {
                    var commentParagraph = new Paragraph(new Run(comment));
                    commentParagraph.SetResourceReference(FrameworkContentElement.StyleProperty, "Comment");
                    yield return commentParagraph;
                }

                foreach (var item in items)
                {
                    var blocks = item.CreateBlocks();

                    if (titlizeFirstParagraph)
                    {
                        var itemTitleParagraph = (Paragraph)blocks[0];
                        itemTitleParagraph.SetResourceReference(FrameworkContentElement.StyleProperty, "Title3");
                    }

                    foreach (var block in blocks)
                        yield return block;
                }

            }
        }

        private void InvokeGeneratePatchnoteDocument()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(this.GeneratePatchnoteDocument));
        }

        private void GeneratePatchnoteDocument()
        {
            var document = new FlowDocument();

            var titleParagraph = new Paragraph(new Run(this.L("patchnote_generator", "generated_document_title", this.TargetRepository.Version)));
            titleParagraph.SetResourceReference(FrameworkContentElement.StyleProperty, "Title");
            document.Blocks.Add(titleParagraph);

            var referenceParagraph = new Paragraph(new Run(this.L("patchnote_generator", "generated_document_reference", this.ReferenceRepository.Version)));
            referenceParagraph.SetResourceReference(FrameworkContentElement.StyleProperty, "ReferenceVersion");
            document.Blocks.Add(referenceParagraph);

            document.Blocks.AddRange(this.CreateTitledList(this.L("patchnote_generator", "generated_document_new_tanks_title"), _addedTanks));
            document.Blocks.AddRange(this.CreateTitledList(this.L("patchnote_generator", "generated_document_obsolete_tanks_title"), _removedTanks));

            document.Blocks.AddRange(this.CreateTitledParagraphs(this.L("patchnote_generator", "generated_document_stat_changes_title"), _statChanges));
            document.Blocks.AddRange(this.CreateTitledParagraphs(this.L("patchnote_generator", "generated_document_potential_armor_changes_title"),
                                                                 _potentialCollisionChanges,
                                                                 this.L("patchnote_generator", "generated_document_potential_armor_changes_footnote")));

            this.Document = document;
        }

        private TypeModifier GetTankTypeModifier(IXQueryable tank)
        {
            return new TypeModifier(this.L("patchnote_generator", "generated_document_tank_type_format", tank["nation"], tank["level"], tank["class"]));
        }

        private void HandleAddedTanks(IXQueryable[] addedTanks)
        {
            foreach (var tank in addedTanks)
            {
                var item = new AddedItem(tank["userString"], tank);
                item.ItemName.AddModifier(this.GetTankTypeModifier(tank));
                _addedTanks.Add(item);

                ++_processedTankTaskCount;
                this.GenerationProgress = (double)_processedTankTaskCount / _tankTaskCount;
            }
        }

        private void HandleRemovedTanks(IXQueryable[] removedTanks)
        {
            foreach (var tank in removedTanks)
            {
                var item = new RemovedItem(tank["userString"], tank);
                item.ItemName.AddModifier(this.GetTankTypeModifier(tank));
                _removedTanks.Add(item);

                ++_processedTankTaskCount;
                this.GenerationProgress = (double)_processedTankTaskCount / _tankTaskCount;
            }
        }

        private void HandleSharedTanks(SharedPair<IXQueryable>[] sharedTanks)
        {
            foreach (var sharedTank in sharedTanks)
            {
                _statChanges.AddIfNotNull(TankDataItemHandler.Instance.CreateReportItem(sharedTank.Source["userString"],
                                                                                        sharedTank.Source,
                                                                                        sharedTank.Target,
                                                                                        false,
                                                                                        false,
                                                                                        false));
                if (_checkPotentialCollisionChanges)
                    this.AnalyseCollisionChanges(sharedTank.Source, sharedTank.Target);

                ++_processedTankTaskCount;
                this.GenerationProgress = (double)_processedTankTaskCount / _tankTaskCount;
            }
        }

        private void AnalyseCollisionChanges(IXQueryable oldTank, IXQueryable newTank)
        {

            var changedParts = new List<ItemName>();
            if (!this.ModelFileEquals(oldTank, newTank, "hull/hitTester/collisionModel"))
                changedParts.Add(new ItemName("hull", false));

            changedParts.AddRange(this.AnalyseModuleCollectionCollisionChanges(oldTank, newTank, "chassis/chassis", "chassis"));
            changedParts.AddRange(this.AnalyseModuleCollectionCollisionChanges(oldTank, newTank, "turrets/turret", "turret"));
            changedParts.AddRange(this.AnalyseModuleCollectionCollisionChanges(oldTank, newTank, "turrets/turret/guns/gun", "gun"));

            if (changedParts.Count > 0)
            {
                _potentialCollisionChanges.Add(new PotentialModelChangedItem(oldTank["userString"], changedParts.ToArray()));
            }
        }

        private IEnumerable<ItemName> AnalyseModuleCollectionCollisionChanges(IXQueryable oldTank, IXQueryable newTank, string xpath, string moduleName)
        {
            var oldModules = oldTank.QueryMany(xpath).Distinct(TankHelper.KeyEqualityComparer);
            var newModules = newTank.QueryMany(xpath).Distinct(TankHelper.KeyEqualityComparer);
            foreach (var sharedItem in oldModules.Diff(newModules, TankHelper.KeyEqualityComparer).Shared)
            {
                if (!this.ModelFileEquals(sharedItem.Source, sharedItem.Target, "hitTester/collisionModel"))
                {
                    var name = new ItemName(sharedItem.Source["userString"]);
                    name.AddModifier(new TypeModifier(moduleName));
                    yield return name;
                }
            }
        }

        private bool ModelFileEquals(IXQueryable oldElement, IXQueryable newElement, string xpath)
        {
            var oldLocalPath = oldElement[xpath];
            var newLocalPath = newElement[xpath];
            return this.PackageFileEquals(oldLocalPath, newLocalPath);
        }

        private bool PackageFileEquals(string oldLocalPath, string newLocalPath)
        {
            var oldPackagePath = this.ReferenceClient.PackageIndexer.GetPackagePath(oldLocalPath);
            var newPackagePath = this.TargetClient.PackageIndexer.GetPackagePath(newLocalPath);

            if (oldPackagePath == null && newPackagePath == null)
                return true;
            else if (oldPackagePath == null || newPackagePath == null)
                return false;


            var oldCrc = PackageStream.GetCrc(oldPackagePath, oldLocalPath);
            var newCrc = PackageStream.GetCrc(newPackagePath, newLocalPath);

            return oldCrc == newCrc;
        }

    }
}
