using System;
using System.Xml.Linq;

using Smellyriver.TankInspector.Pro.Data.Entities;
using Smellyriver.TankInspector.Pro.Data.Tank.Scripting;
using Smellyriver.TankInspector.Pro.Repository;
using TankEntity = Smellyriver.TankInspector.Pro.Data.Entities.Tank;

namespace Smellyriver.TankInspector.Pro.Data.Tank
{
    public class CustomizationConfiguration : ConfigurationBase
    {
        private CustomizationConfigurationInfo _customizationConfigurationInfo;

        internal const string CamouflageElementName = "camouflage";
        internal const string InscriptionElementName = "inscription";

        public CustomizationConfigurationInfo CustomizationConfigurationInfo
        {
            get { return _customizationConfigurationInfo; }
            set
            {
                _customizationConfigurationInfo = value;

                var database = NationalCustomizationDatabase.GetDatabase(this.Repository, this.Tank.NationKey);

                if (_customizationConfigurationInfo.CamouflageId != -1)
                    if (!database.Camouflages.TryGetValue(_customizationConfigurationInfo.CamouflageId, out _camouflage))
                    {
                        Core.Support.LogError(this,
                                              string.Format("cannot find camouflage with id={0}",
                                                            _customizationConfigurationInfo
                                                                .CamouflageId));
                    }

                if (_customizationConfigurationInfo.InscriptionId != -1)
                    if (!database.Inscriptions.TryGetValue(_customizationConfigurationInfo.InscriptionId, out _inscription))
                    {
                        Core.Support.LogError(this,
                                              string.Format("cannot find inscription with id={0}",
                                                            _customizationConfigurationInfo
                                                                .InscriptionId));
                    }
            }
        }

        public event EventHandler CamouflageElementChanged;

        private XElement _camouflageElement;

        internal XElement CamouflageElement
        {
            get { return _camouflageElement; }
            private set
            {
                _camouflageElement = value;

                if (this.CamouflageElementChanged != null)
                    this.CamouflageElementChanged(this, EventArgs.Empty);
            }
        }


        private Camouflage _camouflage;
        public Camouflage Camouflage
        {
            get { return _camouflage; }
            set
            {
                _camouflage = value;
                _customizationConfigurationInfo.CamouflageId = _camouflage == null
                                                             ? CustomizationConfigurationInfo.NoCamouflage
                                                             : _camouflage.Id;

                this.CamouflageElement = this.CreateVehicleSpecifiedCamouflageElement();
            }
        }

        private XElement CreateVehicleSpecifiedCamouflageElement()
        {
            if (this.Camouflage == null)
                return null;

            var element = this.Camouflage.ToElement();

            this.HandleVehicleSpecificCamouflageColor(element, "tiling");
            this.HandleVehicleSpecificCamouflageColor(element, "metallic");
            this.HandleVehicleSpecificCamouflageColor(element, "gloss");

            return element;
        }

        private void HandleVehicleSpecificCamouflageColor(XElement element, string elementName)
        {
            var colorsElement = element.Element(elementName);
            var tankColorElement = colorsElement.Element(this.Tank.Key)
                                ?? colorsElement.Element(XName.Get(this.Tank.Key, this.Tank.NationKey));
            if (tankColorElement == null)
                colorsElement.Remove();
            else
            {
                tankColorElement.Name = elementName;
                colorsElement.ReplaceWith(tankColorElement);
            }
        }

        public event EventHandler InscriptionElementChanged;

        private XElement _inscriptionElement;

        internal XElement InscriptionElement
        {
            get { return _inscriptionElement; }
            private set
            {
                _inscriptionElement = value;
                if (this.InscriptionElementChanged != null)
                    this.InscriptionElementChanged(this, EventArgs.Empty);
            }
        }


        private Inscription _inscription;

        public Inscription Inscription
        {
            get { return _inscription; }
            set
            {
                _inscription = value;
                _customizationConfigurationInfo.InscriptionId = _inscription == null
                                                             ? CustomizationConfigurationInfo.NoInscription
                                                             : _inscription.Id;

                this.InscriptionElement = _inscription == null ? null : _inscription.ToElement();
            }
        }


        internal CustomizationConfiguration(IRepository repository, TankEntity tank, ScriptHost scriptHost, CustomizationConfigurationInfo configInfo)
            : base(repository, tank, scriptHost)
        {
            this.CustomizationConfigurationInfo = configInfo ?? new CustomizationConfigurationInfo();
        }
    }
}
