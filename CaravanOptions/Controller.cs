using System;
using UnityEngine;
using Verse;

namespace CaravanOptions
{
    public class Controller : Mod
    {
        public static Controller Instance;

        private string caravanSpeedMultiplierBuffer = "001.0";
        private bool reset = false;

        public override string SettingsCategory()
        {
            return "Caravan Options";
        }

        public Controller(ModContentPack content) : base(content)
		{
            Controller.Instance = this;
            Settings.Instance = base.GetSettings<Settings>();
            ModCheck.Validate.RoadsOfTheRim.IsInitialized();
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            int num = 0;
            float rowHeight = 36f;
            Rect rect1 = new Rect(canvas);
            rect1.width /= 2f;
            num++;
            Rect rowRect = UIHelper.GetRowRect(rect1, rowHeight, num);
            Text.Font = GameFont.Small;            
            Rect rowRect1 = UIHelper.GetRowRect(rowRect, rowHeight, num);
            rowRect1.x = rowRect.x; // + rect1.width;
            Widgets.TextFieldNumericLabeled<float>(rowRect1, "CaravanSpeedMultiplier".Translate(), ref Settings.Instance.caravanSpeedMultiplier, ref this.caravanSpeedMultiplierBuffer, .1f, 100f);
            Rect rowRect1ShiftedRight = UIHelper.GetRowRect(rowRect1, rowHeight, num);
            rowRect1ShiftedRight.x = rowRect1.x + rect1.width + 4f;
            GUI.contentColor = Color.yellow;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rowRect1ShiftedRight, "CO_SettingsNote".Translate());
            num+=2;
            GUI.contentColor = Color.white;
            Text.Font = GameFont.Small;
            Rect rowRect1a = UIHelper.GetRowRect(rowRect1, rowHeight, num);
            Widgets.Label(rowRect1a, "CO_RoadMultiplierDesc".Translate());
            num++;
            Rect rowRect2 = UIHelper.GetRowRect(rowRect1a, rowHeight, num);
            Settings.Instance.dirtPathCost = Widgets.HorizontalSlider(rowRect2, Settings.Instance.dirtPathCost, .01f, 1f, false, "DirtPathCost".Translate() + " " + Settings.Instance.dirtPathCost, ".01", "1", .01f);

            num++;
            Rect rowRect3 = UIHelper.GetRowRect(rowRect2, rowHeight, num);
            Settings.Instance.dirtRoadCost = Widgets.HorizontalSlider(rowRect3, Settings.Instance.dirtRoadCost, .01f, 1f, false, "DirtRoadCost".Translate() + " " + Settings.Instance.dirtRoadCost, ".01", "1", .01f);
            num++;
            Rect rowRect4 = UIHelper.GetRowRect(rowRect3, rowHeight, num);
            Settings.Instance.stoneRoadCost = Widgets.HorizontalSlider(rowRect4, Settings.Instance.stoneRoadCost, .01f, 1f, false, "StoneRoadCost".Translate() + " " + Settings.Instance.stoneRoadCost, ".01", "1", .01f);
            num++;
            Rect rowRect5 = UIHelper.GetRowRect(rowRect4, rowHeight, num);
            Settings.Instance.asphaltRoadCost = Widgets.HorizontalSlider(rowRect5, Settings.Instance.asphaltRoadCost, .01f, 1f, false, "AsphaltRoadCost".Translate() + " " + Settings.Instance.asphaltRoadCost, ".01", "1", .01f);
            num++;
            Rect rowRect6 = UIHelper.GetRowRect(rowRect5, rowHeight, num);
            Settings.Instance.asphaltHighwayCost = Widgets.HorizontalSlider(rowRect6, Settings.Instance.asphaltHighwayCost, .01f, 1f, false, "AsphaltHighwayCost".Translate() + " " + Settings.Instance.asphaltHighwayCost, ".01", "1", .01f);
            Rect rowRect61 = new Rect();
            if (ModCheck.Validate.RoadsOfTheRim.IsInitialized())
            {
                num++;
                rowRect61 = UIHelper.GetRowRect(rowRect6, rowHeight, num);
                Settings.Instance.glitterRoadCost = Widgets.HorizontalSlider(rowRect61, Settings.Instance.glitterRoadCost, .01f, 1f, false, "GlitterRoadCost".Translate() + " " + Settings.Instance.glitterRoadCost, ".01", "1", .01f);
            }
            num += 2;
            Rect rowRect7 = new Rect();
            if(ModCheck.Validate.RoadsOfTheRim.IsInitialized())                
            {
                rowRect7 = UIHelper.GetRowRect(rowRect61, rowHeight, num);
            }
            else
            {
                rowRect7 = UIHelper.GetRowRect(rowRect6, rowHeight, num);
            }
            Settings.Instance.massUsageBonus = Widgets.HorizontalSlider(rowRect7, Settings.Instance.massUsageBonus, 0.1f, 5f, false, "CO_MassUsageBonus".Translate() + " " + Settings.Instance.massUsageBonus, ".1", "5", .1f);
            num++;
            Rect rowRect9 = UIHelper.GetRowRect(rowRect7, rowHeight, num);
            Settings.Instance.massCapUpperLimit = Widgets.HorizontalSlider(rowRect9, Settings.Instance.massCapUpperLimit, 0.1f, 5f, false, "CO_ImmobilizeCap".Translate(Settings.Instance.massCapUpperLimit.ToString("0.#")), "1", "5", .1f);
            Rect rowRect9ShiftedRight = UIHelper.GetRowRect(rowRect9, rowHeight, num);
            rowRect9ShiftedRight.x = rowRect9.x + rect1.width + 4f;
            GUI.contentColor = Color.yellow;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rowRect9ShiftedRight, "CO_ImmobilizeCapNote".Translate());
            GUI.contentColor = Color.white;
            Text.Font = GameFont.Small;
            num++;
            Rect rowRect10 = UIHelper.GetRowRect(rowRect9, rowHeight, num);
            Settings.Instance.foragingMultiplier = Widgets.HorizontalSlider(rowRect10, Settings.Instance.foragingMultiplier, 0.1f, 5f, false, "CO_ForagingMultiplier".Translate() + " " + Settings.Instance.foragingMultiplier, "1", "5", .1f);

            num++;
            GUI.contentColor = Color.yellow;
            Text.Font = GameFont.Small;
            Rect rowRect11 = UIHelper.GetRowRect(rowRect10, rowHeight, num);
            rowRect11.x += +rect1.width - 36f;
            rowRect11.y += 18f;
            rowRect11.width = 64f;
            reset = Widgets.ButtonText(rowRect11, "RESET", true, false, true);
            if (reset)
            {
                Settings.Instance.caravanSpeedMultiplier = 1f;
                this.caravanSpeedMultiplierBuffer = "001.0";
                Settings.Instance.dirtPathCost = 0.5f;
                Settings.Instance.dirtRoadCost = 0.5f;
                Settings.Instance.stoneRoadCost = 0.5f;
                Settings.Instance.asphaltRoadCost = 0.5f;
                Settings.Instance.asphaltHighwayCost = 0.5f;
                Settings.Instance.foragingMultiplier = 1f;
                Settings.Instance.massUsageBonus = 2f;
                Settings.Instance.massCapUpperLimit = 1f;
                Settings.Instance.glitterRoadCost = .25f;
            }
        }

        public static class UIHelper
        {
            public static Rect GetRowRect(Rect inRect, float rowHeight, int row)
            {
                float y = rowHeight * (float)row;
                Rect result = new Rect(inRect.x, y, inRect.width, rowHeight);
                return result;
            }
        }
    }
}
