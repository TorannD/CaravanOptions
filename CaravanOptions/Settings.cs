using System;
using Verse;

namespace CaravanOptions
{
    public class Settings : ModSettings
    {
        public static Settings Instance;

        public float caravanSpeedMultiplier = 1f;
        public float dirtPathCost = .5f;
        public float dirtRoadCost = .5f;
        public float stoneRoadCost = .5f;
        public float asphaltRoadCost = .5f;
        public float asphaltHighwayCost = .5f;
        public float foragingMultiplier = 1f;
        public float massUsageBonus = 2f;
        public float massCapUpperLimit = 1f;

        //roads of the rim check
        public float glitterRoadCost = .25f;

        public Settings()
        {
            Settings.Instance = this;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look<float>(ref this.caravanSpeedMultiplier, "caravanSpeedMultiplier", 1f, false);
            Scribe_Values.Look<float>(ref this.dirtPathCost, "dirtPathCost", 0.5f, false);
            Scribe_Values.Look<float>(ref this.dirtRoadCost, "dirtRoadCost", 0.5f, false);
            Scribe_Values.Look<float>(ref this.stoneRoadCost, "stoneRoadCost", 0.5f, false);
            Scribe_Values.Look<float>(ref this.asphaltRoadCost, "asphaltRoadCost", 0.5f, false);
            Scribe_Values.Look<float>(ref this.asphaltHighwayCost, "asphaltHighwayCost", 0.5f, false);
            Scribe_Values.Look<float>(ref this.foragingMultiplier, "foragingMultiplier", 1f, false);
            Scribe_Values.Look<float>(ref this.massUsageBonus, "massUsageBonus", 2f, false);
            Scribe_Values.Look<float>(ref this.massCapUpperLimit, "massCapUpperLimit", 1f, false);
            //roads of the rim
            Scribe_Values.Look<float>(ref this.glitterRoadCost, "glitterRoadCost", 0.25f, false);

        }
    }
}
