using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace CaravanOptions
{
    public class CompCaravanOptions : WorldObjectComp
    {
        public bool forceNightMove = false;

        public Caravan ParentCaravan
        {
            get
            {
                Caravan caravan = this.parent as Caravan;
                bool flag = caravan == null || !(caravan is Caravan);
                if (flag)
                {
                    Log.Error("caravan is null");
                }
                return caravan;
            }
        }

        public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        {          
            var gizmoList = base.GetCaravanGizmos(caravan).ToList();

            if (this.parent != null && this.parent.Faction != null && this.parent.Faction == Faction.OfPlayerSilentFail)
            {
                String label = "CO_ForceNightMove".Translate();
                String desc = "CO_ForceNightMoveDesc".Translate();
                Command_Toggle item = new Command_Toggle
                {
                    defaultLabel = label,
                    defaultDesc = desc,
                    order = 199,
                    icon = ContentFinder<Texture2D>.Get("UI/ForceNight", true),
                    isActive = (() => this.forceNightMove),
                    toggleAction = delegate
                    {
                        this.forceNightMove = !this.forceNightMove;
                    }
                };

                gizmoList.Add(item);
            }
            return gizmoList;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.forceNightMove, "forceNightMove", false, false);
        }

    }
}
