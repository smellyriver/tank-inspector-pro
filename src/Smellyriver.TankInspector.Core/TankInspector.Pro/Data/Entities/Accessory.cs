using System.Globalization;
using System.Linq;

namespace Smellyriver.TankInspector.Pro.Data.Entities
{
    public abstract class Accessory : Component
    {

        public static bool IsCompatible(Accessory accessory1, Accessory accessory2)
        {
            return Accessory.InternalIsCompatible(accessory1, accessory2)
                && Accessory.InternalIsCompatible(accessory2, accessory1);
        }

        private static bool InternalIsCompatible(Accessory accessory1, Accessory accessory2)
        {
            var incompatibleTags = accessory1.QueryManyValues("incompatibleTags/installed");
            foreach (var tag in incompatibleTags)
            {
                if (accessory2.QueryManyValues("tags/tag").Contains(tag))
                    return false;
            }

            return true;
        }

        public string Description { get { return this["description"]; } }

        public string IconKey { get { return this["icon"]; } }


        protected Accessory(IXQueryable data)
            : base(data)
        {

        }


        public bool CanBeUsedBy(Tank tank)
        {
            var tankNation = tank.NationKey;
            if (this.QueryManyValues("vehicleFilter/exclude/nations/nation").Contains(tankNation))
                return false;

            var includeNations = this.Query("vehicleFilter/include/nations");
            if (includeNations != null)
            {
                if (!includeNations.QueryManyValues("nation").Contains(tankNation))
                    return false;
            }

            var tankTags = tank.QueryManyValues("tags/tag").ToArray();
            foreach (var tag in this.QueryManyValues("vehicleFilter/exclude/vehicle/tags"))
            {
                if (tankTags.Contains(tag))
                    return false;
            }

            var includeVehicleTags = this.Query("vehicleFilter/include/vehicle/tags");

            if (includeVehicleTags != null)
            {
                var matched = false;
                foreach (var tag in includeVehicleTags.QueryManyValues("tag"))
                {
                    if (tankTags.Contains(tag))
                    {
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                    return false;
            }

            var maxLevelString = this["vehicleFilter/include/vehicle/maxLevel"];
            if (maxLevelString != null)
            {
                var maxLevel = int.Parse(maxLevelString, CultureInfo.InvariantCulture);
                if (tank.Tier > maxLevel)
                    return false;
            }

            var minLevelString = this["vehicleFilter/include/vehicle/minLevel"];
            if (minLevelString != null)
            {
                var minLevel = int.Parse(minLevelString, CultureInfo.InvariantCulture);
                if (tank.Tier < minLevel)
                    return false;
            }

            var engineTags = tank.QueryManyValues("engines/engine/tags");
            foreach (var tag in this.QueryManyValues("vehicleFilter/exclude/engine/tags"))
            {
                if (engineTags.Contains(tag))
                    return false;
            }

            var includeEngineTags = this.Query("vehicleFilter/include/engine/tags");
            if (includeEngineTags != null)
            {
                var matched = false;
                foreach (var tag in includeEngineTags.QueryManyValues("tag"))
                {
                    if (engineTags.Contains(tag))
                    {
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                    return false;
            }


            return true;
        }


    }
}
