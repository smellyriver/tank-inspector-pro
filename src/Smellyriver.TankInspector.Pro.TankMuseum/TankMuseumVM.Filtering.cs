using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using System.Xml.XPath;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Gameplay;
using Smellyriver.TankInspector.Pro.Repository;
using Smellyriver.TankInspector.Pro.UserInterface.Design;
using Localization = Smellyriver.TankInspector.Pro.Globalization.Localization;

namespace Smellyriver.TankInspector.Pro.TankMuseum
{
    partial class TankMuseumVM
    {
        private static readonly ComboBoxItemVM<FilterMode> s_keywordModeVm
            = ComboBoxItemVM.Create(FilterMode.Keyword,
                                    Localization.Instance.L("tank_museum", "filter_mode_keyword"),
                                    BitmapImageEx.LoadAsFrozen("Resources/Images/Filter_16.png"));

        private static readonly ComboBoxItemVM<FilterMode> s_xpathModeVm
            = ComboBoxItemVM.Create(FilterMode.XPath,
                                    Localization.Instance.L("tank_museum", "filter_mode_xpath"),
                                    BitmapImageEx.LoadAsFrozen("Resources/Images/Query_16.png"));

        private static readonly ComboBoxItemVM<IRepository> s_allRepositoriesVm
            = ComboBoxItemVM.Create<IRepository>(null,
                                                 Localization.Instance.L("tank_museum", "filter_all_game_clients"),
                                                 BitmapImageEx.LoadAsFrozen("Resources/Images/RepositoryColumn_27.png"));

        private static readonly ComboBoxItemVM<string> s_allClassesVm
            = ComboBoxItemVM.Create("all",
                                    Localization.Instance.L("tank_museum", "filter_all_tank_classes"),
                                    BitmapImageEx.LoadAsFrozen("Resources/Images/ClassesColumn_27.png"));

        private static readonly ComboBoxItemVM<string> s_allNationsVm
            = ComboBoxItemVM.Create("all",
                                    Localization.Instance.L("tank_museum", "filter_all_nations"),
                                    BitmapImageEx.LoadAsFrozen("Resources/Images/NationsColumn_27.png"));

        private static readonly ComboBoxItemVM<int> s_allTiersVm
            = ComboBoxItemVM.Create<int>(0,
                                         Localization.Instance.L("tank_museum", "filter_all_tiers"),
                                         BitmapImageEx.LoadAsFrozen("Resources/Images/TiersColumn_27.png"));

        private static readonly ComboBoxItemVM<PremiumState> s_premiumVm
            = ComboBoxItemVM.Create(PremiumState.Premium,
                                    Localization.Instance.L("tank_museum", "filter_premium"),
                                    BitmapImageEx.LoadAsFrozen("Resources/Images/Premium_27.png"));

        private static readonly ComboBoxItemVM<PremiumState> s_nonPremiumVm
            = ComboBoxItemVM.Create(PremiumState.NonPremium,
                                    Localization.Instance.L("tank_museum", "filter_non_premium"),
                                    BitmapImageEx.LoadAsFrozen("Resources/Images/NonPremium_27.png"));

        private static readonly ComboBoxItemVM<PremiumState> s_bothPremiumVm
            = ComboBoxItemVM.Create(PremiumState.Both,
                                    Localization.Instance.L("tank_museum", "filter_both_premium"),
                                    BitmapImageEx.LoadAsFrozen("Resources/Images/PremiumBoth_27.png"));


        private readonly ComboBoxItemVM<FilterMode>[] _filterModes = new[] { s_keywordModeVm, s_xpathModeVm };
        public IEnumerable<ComboBoxItemVM<FilterMode>> FilterModes { get { return _filterModes; } }

        private string _filterText;
        public string FilterText
        {
            get { return _filterText; }
            set
            {
                _filterText = value;
                this.RaisePropertyChanged(() => this.FilterText);
                this.RaisePropertyChanged(() => this.FilterKeyword);
                this.OnFilterTextChanged();
            }
        }

        public string FilterKeyword
        {
            get { return this.SelectedFilterMode.Model == FilterMode.Keyword ? this.FilterText : null; }
        }

        private bool _isFilterTextInvalid;
        public bool IsFilterTextInvalid
        {
            get { return _isFilterTextInvalid; }
            private set
            {
                _isFilterTextInvalid = value;
                this.RaisePropertyChanged(() => this.IsFilterTextInvalid);
            }
        }


        public ComboBoxItemVM<FilterMode> SelectedFilterMode
        {
            get { return _filterModes[TankMuseumSettings.Default.FilterMode.Clamp(0, 1)]; }
            set
            {
                if (value != this.SelectedFilterMode)
                {
                    var filterMode = Array.IndexOf(_filterModes, value);
                    if (filterMode == -1)
                        filterMode = 0;
                    TankMuseumSettings.Default.FilterMode = filterMode;
                    TankMuseumSettings.Default.Save();
                    this.RaisePropertyChanged(() => this.SelectedFilterMode);
                    this.RaisePropertyChanged(() => this.IsInKeywordMode);
                    this.RaisePropertyChanged(() => this.FilterTextBoxWatermark);
                    this.RaisePropertyChanged(() => this.FilterKeyword);
                    this.OnFilterModeChanged();
                }
            }
        }

        public bool IsInKeywordMode
        {
            get { return this.SelectedFilterMode.Model == FilterMode.Keyword; }
        }

        public string FilterTextBoxWatermark
        {
            get
            {
                return Localization.Instance.L("tank_museum",
                                               this.IsInKeywordMode
                                               ? "filter_mode_keyword"
                                               : "filter_mode_xpath",
                                               Assembly.GetExecutingAssembly());
            }
        }

        private ComboBoxItemVM<string>[] _nationFilters;
        public IEnumerable<ComboBoxItemVM<string>> NationFilters
        {
            get { return _nationFilters; }
            private set
            {
                _nationFilters = value as ComboBoxItemVM<string>[];
                this.RaisePropertyChanged(() => this.NationFilters);
                this.FilterTanks();
            }
        }



        public ComboBoxItemVM<string> SelectedNationFilter
        {
            get
            {
                if (_nationFilters == null)
                    return s_allNationsVm;

                return _nationFilters.FirstOrDefault(c => c.Model == TankMuseumSettings.Default.NationFilter)
                    ?? s_allNationsVm;
            }
            set
            {
                value = value ?? s_allNationsVm;
                TankMuseumSettings.Default.NationFilter = value.Model;
                TankMuseumSettings.Default.Save();
                this.RaisePropertyChanged(() => this.SelectedNationFilter);
                this.FilterTanks();
            }
        }


        private ComboBoxItemVM<string>[] _classFilters;
        public IEnumerable<ComboBoxItemVM<string>> ClassFilters
        {
            get { return _classFilters; }
            private set
            {
                _classFilters = value as ComboBoxItemVM<string>[];
                this.RaisePropertyChanged(() => this.ClassFilters);
                this.FilterTanks();
            }
        }

        public ComboBoxItemVM<string> SelectedClassFilter
        {
            get
            {
                if (_classFilters == null)
                    return s_allClassesVm;

                return _classFilters.FirstOrDefault(c => c.Model == TankMuseumSettings.Default.ClassFilter)
                    ?? s_allClassesVm;
            }
            set
            {
                value = value ?? s_allClassesVm;
                TankMuseumSettings.Default.ClassFilter = value.Model;
                TankMuseumSettings.Default.Save();
                this.RaisePropertyChanged(() => this.SelectedClassFilter);
                this.FilterTanks();
            }
        }


        private ComboBoxItemVM<int>[] _tierFilters;
        public IEnumerable<ComboBoxItemVM<int>> TierFilters
        {
            get { return _tierFilters; }
            private set
            {
                _tierFilters = value as ComboBoxItemVM<int>[];
                this.RaisePropertyChanged(() => this.TierFilters);
                this.FilterTanks();
            }
        }

        public ComboBoxItemVM<int> SelectedTierFilter
        {
            get
            {
                if (_tierFilters == null)
                    return s_allTiersVm;

                return _tierFilters.FirstOrDefault(c => c.Model == TankMuseumSettings.Default.TierFilter)
                    ?? s_allTiersVm;
            }
            set
            {
                value = value ?? s_allTiersVm;
                TankMuseumSettings.Default.TierFilter = value.Model;
                TankMuseumSettings.Default.Save();
                this.RaisePropertyChanged(() => this.SelectedTierFilter);
                this.FilterTanks();
            }
        }


        private ComboBoxItemVM<IRepository>[] _repositoryFilters;
        public IEnumerable<ComboBoxItemVM<IRepository>> RepositoryFilters
        {
            get { return _repositoryFilters; }
            private set
            {
                _repositoryFilters = value as ComboBoxItemVM<IRepository>[];
                this.RaisePropertyChanged(() => this.RepositoryFilters);
                this.FilterTanks();
            }
        }

        public ComboBoxItemVM<IRepository> SelectedRepositoryFilter
        {
            get
            {
                if (string.IsNullOrEmpty(TankMuseumSettings.Default.RepositoryFilter))
                    return s_allRepositoriesVm;

                if (_repositoryFilters == null)
                    return s_allRepositoriesVm;

                return _repositoryFilters.FirstOrDefault(c => c.Model != null && c.Model.ID == TankMuseumSettings.Default.RepositoryFilter)
                    ?? s_allRepositoriesVm;
            }
            set
            {
                value = value ?? s_allRepositoriesVm;

                TankMuseumSettings.Default.RepositoryFilter = value.Model == null
                                                            ? "all"
                                                            : value.Model.ID;
                TankMuseumSettings.Default.Save();
                this.RaisePropertyChanged(() => this.SelectedRepositoryFilter);
                this.FilterTanks();
            }
        }


        private readonly ComboBoxItemVM<PremiumState>[] _premiumFilters = new[] { s_bothPremiumVm, s_nonPremiumVm, s_premiumVm };
        public IEnumerable<ComboBoxItemVM<PremiumState>> PremiumFilters { get { return _premiumFilters; } }

        public ComboBoxItemVM<PremiumState> SelectedPremiumFilter
        {
            get { return _premiumFilters[TankMuseumSettings.Default.PremiumFilter.Clamp(0, 2)]; }
            set
            {
                var premiumFilter = Array.IndexOf(_premiumFilters, value);
                if (premiumFilter == -1)
                    premiumFilter = 0;
                TankMuseumSettings.Default.PremiumFilter = premiumFilter;
                TankMuseumSettings.Default.Save();
                this.RaisePropertyChanged(() => this.SelectedPremiumFilter);
                this.FilterTanks();
            }
        }


        private Dictionary<IRepository, HashSet<IXQueryable>> _xpathQueryResult;
        private DispatcherTimer _deferFilterTextChangesTimer;

        private bool _isInitializingFilters = false;

        private void ResetFilters()
        {
            _isInitializingFilters = true;
            this.SelectedClassFilter = s_allClassesVm;
            this.SelectedNationFilter = s_allNationsVm;
            this.SelectedRepositoryFilter = s_allRepositoriesVm;
            this.SelectedPremiumFilter = s_bothPremiumVm;
            this.SelectedTierFilter = s_allTiersVm;
            _isInitializingFilters = false;
            this.FilterTanks();
        }

        private string GetCompositeLocalization(Func<IRepositoryLocalization, string> getter)
        {
            return string.Join("/",
                               RepositoryManager.Instance.Repositories.Select(r => getter(r.Localization)).Distinct());
        }

        private void UpdateFilters()
        {
            var tanks = _allTanks.SelectMany(t => t).Select(t => t.Model);

            var anyClient = RepositoryManager.Instance.Repositories.OfType<LocalGameClient>().FirstOrDefault();

            var repositories = RepositoryManager.Instance.Repositories.OrderBy(r => r.Version).ToArray();
            var repositoryFilters = new ComboBoxItemVM<IRepository>[repositories.Length + 1];
            repositoryFilters[0] = s_allRepositoriesVm;
            for (var i = 0; i < repositories.Length; ++i)
            {
                var repository = repositories[i];
                repositoryFilters[i + 1] = new RepositoryComboBoxItemVM(repository);
            }

            this.RepositoryFilters = repositoryFilters;
            this.RaisePropertyChanged(() => this.SelectedRepositoryFilter);

            var tiers = tanks.Select(t => t.Tier).Distinct().OrderBy(t => t).ToArray();
            var tierFilters = new ComboBoxItemVM<int>[tiers.Length + 1];
            tierFilters[0] = s_allTiersVm;
            for (var i = 0; i < tiers.Length; ++i)
            {
                var tier = tiers[i];
                tierFilters[i + 1] = new ComboBoxItemVM<int>(tier, RomanNumberService.GetRomanNumber(tier), null);
            }
            this.TierFilters = tierFilters;
            this.RaisePropertyChanged(() => this.SelectedTierFilter);

            var classKeys = tanks.Select(t => t.ClassKey).Distinct().OrderBy(TankClassComparer.GetClassSortIndex).ToArray();
            var classFilters = new ComboBoxItemVM<string>[classKeys.Length + 1];
            classFilters[0] = s_allClassesVm;
            for (var i = 0; i < classKeys.Length; ++i)
            {
                var classKey = classKeys[i];
                classFilters[i + 1] = new ComboBoxItemVM<string>(classKey,
                                                                 this.GetCompositeLocalization(r => r.GetLocalizedClassName(classKey)),
                                                                 anyClient == null ? null : anyClient.PackageImages.GetClassSmallIcon(classKey));
            }
            this.ClassFilters = classFilters;
            this.RaisePropertyChanged(() => this.SelectedClassFilter);

            var nations = tanks.Select(t => t.NationKey).Distinct().OrderBy(n => n).ToArray();
            var nationFilters = new ComboBoxItemVM<string>[nations.Length + 1];
            nationFilters[0] = s_allNationsVm;
            for (var i = 0; i < nations.Length; ++i)
            {
                var @nation = nations[i];
                nationFilters[i + 1] = new ComboBoxItemVM<string>(@nation,
                                                                  this.GetCompositeLocalization(r => r.GetLocalizedNationName(@nation)),
                                                                  anyClient == null ? null : anyClient.PackageImages.GetNationSmallIcon(@nation));
            }
            this.NationFilters = nationFilters;
            this.RaisePropertyChanged(() => this.SelectedNationFilter);
        }


        private void FilterTanks()
        {
            if (_isInitializingFilters)
                return;

            this.ListedTanks.Refresh();
        }

        private bool Filter(object target)
        {
            var tank = target as TankVM;
            if (tank == null)
                return false;

            if (this.SelectedFilterMode.Model == FilterMode.Keyword)
                return this.KeywordFilter(tank);
            else
                return this.XPathFilter(tank);
        }


        private void OnFilterTextChanged()
        {
            if (_deferFilterTextChangesTimer != null)
            {
                _deferFilterTextChangesTimer.Stop();
            }

            _deferFilterTextChangesTimer = new DispatcherTimer(TimeSpan.FromSeconds(0.2),
                                                               DispatcherPriority.Normal,
                                                               (o, e) => this.ReflectFilterTextChanges(),
                                                               Application.Current.Dispatcher);

            _deferFilterTextChangesTimer.Start();
        }

        private void ReflectFilterTextChanges()
        {
            _deferFilterTextChangesTimer.Stop();
            _deferFilterTextChangesTimer = null;

            if (this.SelectedFilterMode.Model == FilterMode.XPath)
                this.UpdateXPathQuery();

            this.FilterTanks();
        }

        private void UpdateXPathQuery()
        {
            _xpathQueryResult = RepositoryManager.Instance.Repositories.ToDictionary(
                r => r,
                r => new HashSet<IXQueryable>(this.FilterRepository(r), TankHelper.KeyEqualityComparer));
        }

        private IEnumerable<IXQueryable> FilterRepository(IRepository repository)
        {
            if (string.IsNullOrWhiteSpace(this.FilterText))
            {
                this.IsFilterTextInvalid = false;
                return repository.TankDatabase.QueryMany("tank");
            }
            else
            {
                try
                {
                    this.IsFilterTextInvalid = false;
                    return repository.TankDatabase.QueryMany(string.Format("tank[{0}]", this.FilterText));
                }
                catch (XPathException)
                {
                    this.IsFilterTextInvalid = true;
                    return new IXQueryable[0];
                }
            }
        }

        private void OnFilterModeChanged()
        {
            if (this.SelectedFilterMode.Model == FilterMode.XPath)
                this.UpdateXPathQuery();

            this.FilterText = "";
        }

        private void ResetFilterText()
        {
            this.IsFilterTextInvalid = false;
            this.FilterText = "";
        }

        private bool RepositoryFilter(TankVM tank)
        {
            if (this.SelectedRepositoryFilter != s_allRepositoriesVm)
                if (tank.Repository.Model != this.SelectedRepositoryFilter.Model)
                    return false;

            return true;
        }

        private bool XPathFilter(TankVM tank)
        {
            if (!this.RepositoryFilter(tank))
                return false;

            if (_xpathQueryResult == null)
                return true;

            var queryResultTanks = _xpathQueryResult[tank.Repository.Model];
            return queryResultTanks.Contains(tank.Model);
        }

        private bool KeywordFilter(TankVM tank)
        {
            if (!this.RepositoryFilter(tank))
                return false;

            if (this.SelectedClassFilter != s_allClassesVm)
                if (tank.Model.ClassKey != this.SelectedClassFilter.Model)
                    return false;

            if (this.SelectedNationFilter != s_allNationsVm)
                if (tank.Model.NationKey != this.SelectedNationFilter.Model)
                    return false;

            if (this.SelectedTierFilter != s_allTiersVm)
                if (tank.Model.Tier != this.SelectedTierFilter.Model)
                    return false;

            if (this.SelectedPremiumFilter != s_bothPremiumVm)
                if (tank.Model.IsPremium != (this.SelectedPremiumFilter.Model == PremiumState.Premium))
                    return false;

            if (string.IsNullOrWhiteSpace(this.FilterText))
                return true;

            return tank.Name.ToLowerInvariant().Contains(this.FilterText.ToLowerInvariant())
                || tank.ShortName.ToLowerInvariant().Contains(this.FilterText.ToLowerInvariant());
        }

    }
}
