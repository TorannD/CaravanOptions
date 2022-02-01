using System;
using Verse;

namespace CaravanOptions
{
    public class SettingsRef
    {
        public float speedMultiplier = Settings.Instance.caravanSpeedMultiplier;
        public bool pawnSpeedMatters = Settings.Instance.pawnSpeedMatters;
        public float dirtPath = Settings.Instance.dirtPathCost;
        public float dirtRoad = Settings.Instance.dirtRoadCost;
        public float stoneRoad = Settings.Instance.stoneRoadCost;
        public float asphaltRoad = Settings.Instance.asphaltRoadCost;
        public float asphaltHighway = Settings.Instance.asphaltHighwayCost;
        public float foragingMultiplier = Settings.Instance.foragingMultiplier;
        public float massUsageBonus = Settings.Instance.massUsageBonus;
        public float massCapUpperLimit = Settings.Instance.massCapUpperLimit;
        //roads of the rim
        public float glitterRoad = Settings.Instance.glitterRoadCost;

        public bool overrideTick = Settings.Instance.overrideTick;
        public int overrideTickHash = Settings.Instance.overrideTickHash;
    }
}
