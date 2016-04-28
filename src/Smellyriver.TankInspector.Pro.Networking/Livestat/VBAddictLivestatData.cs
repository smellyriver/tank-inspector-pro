using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Pro.Networking.Livestat
{
    [DataContract(Name = "VBAddictLivestat", Namespace = "http://schemas.smellyriver.com/stipro/stats")]
    public class VBAddictLivestatData
    {
        [DataMember]
        public double damage_dealt { get; set; }
        [DataMember]
        public double potential_damage_received { get; set; }
        [DataMember]
        public double damage_received { get; set; }
        [DataMember]
        public double damage_assisted_radio { get; set; }
        [DataMember]
        public double damage_assisted_track { get; set; }
        [DataMember]
        public double accuracy { get; set; }
        [DataMember]
        public double experience { get; set; }
        [DataMember]
        public double creditsb { get; set; }
        [DataMember]
        public double operating_costs { get; set; }
        [DataMember]
        public double equip_costs { get; set; }
        [DataMember]
        public double ammunition_costs { get; set; }
        [DataMember]
        public double repair_costs { get; set; }
        [DataMember]
        public double creditsn { get; set; }

        //public double crits_fire { get; set; }
        [DataMember]
        public double mileage { get; set; }
        [DataMember]
        public double spotted { get; set; }
        [DataMember]
        public double winrate { get; set; }
        [DataMember]
        public double damaged { get; set; }
        [DataMember]
        public double efficiency_wnx { get; set; }
        [DataMember]
        public double efficiency_wn7 { get; set; }
        [DataMember]
        public double efficiency_wn8 { get; set; }
        [DataMember]
        public double efficiency_box { get; set; }
        [DataMember]
        public double killed { get; set; }
        [DataMember]
        public double battle_time { get; set; }
        [DataMember]
        public double survived { get; set; }
        [DataMember]
        public double frequency_of_occurrence { get; set; }
        [DataMember]
        public double experience_per_minute { get; set; }
        [DataMember]
        public double penetration_rate { get; set; }
        [DataMember]
        public double nodamageshotsratio { get; set; }
        [DataMember]
        public double kamikaze { get; set; }
        [DataMember]
        public double sleeping { get; set; }
        [DataMember]
        public double mm_tier_weight { get; set; }
        //public double crits_engine { get; set; }
        //public double crits_ammoBay { get; set; }
        //public double crits_fuelTank { get; set; }
        //public double crits_radio { get; set; }
        //public double crits_track { get; set; }
        //public double crits_gun { get; set; }
        //public double crits_turretRotator { get; set; }
        //public double crits_surveyingDevice { get; set; }
        //public double crits_commander { get; set; }
        //public double crits_driver { get; set; }
        //public double crits_radioman { get; set; }
        //public double crits_gunner { get; set; }
        //public double crits_loader { get; set; }
        //public double crits_destroyed_engine { get; set; }
        //public double crits_destroyed_ammoBay { get; set; }
        //public double crits_destroyed_fuelTank { get; set; }
        //public double crits_destroyed_radio { get; set; }
        //public double crits_destroyed_track { get; set; }
        //public double crits_destroyed_gun { get; set; }
        //public double crits_destroyed_turretRotator { get; set; }
        //public double crits_destroyed_surveyingDevice { get; set; }
        //public double death_shot { get; set; }
        //public double death_fire { get; set; }
        //public double death_ramming { get; set; }
        //public double death_crashed { get; set; }
        //public double death_drowned { get; set; }
    }
}
