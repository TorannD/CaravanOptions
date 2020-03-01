using HarmonyLib;
using System;
using System.Text;
using System.Reflection;
using Verse;
using RimWorld.Planet;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CaravanOptions
{

    [StaticConstructorOnStartup]
    internal class Main
    {
        static Main()
        {
            var harmonyInstance = new Harmony("rimworld.torann.CaravanOptions");
            harmonyInstance.Patch(AccessTools.Method(typeof(Caravan), "get_ImmobilizedByMass", null, null), null, new HarmonyMethod(typeof(Main), "Get_ImmobilizedByMass"), null);
            harmonyInstance.Patch(AccessTools.Method(typeof(Caravan), "get_NightResting", null, null), null, new HarmonyMethod(typeof(Main), "Get_NightResting_Forced"), null);
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void Get_NightResting_Forced(Caravan __instance, ref bool __result)
        {
            CompCaravanOptions comp = __instance.GetComponent<CompCaravanOptions>();
            if(comp != null && comp.forceNightMove)
            {                
                __result = false;
            }
        }

        public static void Get_ImmobilizedByMass(Caravan __instance, ref bool __result)
        {
            int cachedImmobilizedForTicks = Traverse.Create(root: __instance).Field(name: "cachedImmobilizedForTicks").GetValue<int>();
            if (Find.TickManager.TicksGame - cachedImmobilizedForTicks < 60)
            {
                SettingsRef settingsRef = new SettingsRef();
                if (settingsRef.massCapUpperLimit != 1f)
                {
                    __result = ((__instance.MassUsage / settingsRef.massCapUpperLimit) > __instance.MassCapacity);
                    Traverse.Create(root: __instance).Field(name: "cachedImmobilized").SetValue(__result);
                    Traverse.Create(root: __instance).Field(name: "cachedImmobilizedForTicks").SetValue(Find.TickManager.TicksGame);
                }
            }
        }

        [HarmonyPatch(typeof(ForagedFoodPerDayCalculator), "GetForagedFoodCountPerInterval", new Type[]
            {
                typeof(List<Pawn>),
                typeof(BiomeDef),
                typeof(Faction),
                typeof(StringBuilder)
            })]
        public class GetForagedFoodCountPerInterval_Patch
        {
            private static void Postfix(ref float __result, StringBuilder explanation = null)
            {
                SettingsRef settingsRef = new SettingsRef();
                __result *= Mathf.RoundToInt(settingsRef.foragingMultiplier);
                if (explanation != null)
                {
                    explanation.AppendLine();
                    explanation.AppendLine();
                    explanation.Append(string.Concat(new string[]
                    {
                        "CO_ForagingMultiplier".Translate(),
                        ": x",
                        settingsRef.foragingMultiplier.ToString("0.##")
                    }));
                }
            }
        }

        [HarmonyPatch(typeof(CaravanTicksPerMoveUtility), "GetMoveSpeedFactorFromMass", null)]
        public class GetMoveSpeedFactorFromMass_Patch
        {
            private static bool Prefix(float massUsage, float massCapacity, ref float __result)
            {
                SettingsRef settingsRef = new SettingsRef();
                if (settingsRef.massCapUpperLimit != 1f && massCapacity <= massUsage)
                {
                    float t = ((massCapacity * settingsRef.massCapUpperLimit) - massUsage) / ((massCapacity * settingsRef.massCapUpperLimit) - massCapacity);
                    __result = t;
                    return false;
                }
                else
                {
                    if (massCapacity > 0f)
                    {
                        float t = massUsage / massCapacity;
                        __result = Mathf.Lerp(settingsRef.massUsageBonus, 1f, t);
                        return false;
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(CaravanArrivalTimeEstimator), "EstimatedTicksToArrive", new Type[]
            {
                typeof(int),
                typeof(int),
                typeof(WorldPath),
                typeof(float),
                typeof(int),
                typeof(int)
            })]
        public class EstimatedTicksToArrive_Base_Patch
        {
            private static void Postfix(ref int __result)
            {
                SettingsRef settingsRef = new SettingsRef();

                __result = Mathf.RoundToInt(__result / settingsRef.speedMultiplier);
            }
        }

        [HarmonyPatch(typeof(Caravan_PathFollower), "CostToPayThisTick", null)]
        public class CostToPayThisTick_Base_Patch
        {
            private static void Postfix(ref float __result)
            {
                SettingsRef settingsRef = new SettingsRef();
                __result *= Mathf.RoundToInt(settingsRef.speedMultiplier);
            }
        }

        [HarmonyPatch(typeof(Caravan_PathFollower), "CostToMove", new Type[]
        {
            typeof(int),
            typeof(int),
            typeof(int),
            typeof(int?),
            typeof(bool),
            typeof(StringBuilder),
            typeof(string)
        })]
        public static class CostToMove_Patch
        {
            [HarmonyPrefix]
            public static bool CostToMove_Prefix(int caravanTicksPerMove, int start, int end, ref int __result, int? ticksAbs = null, bool perceivedStatic = false, StringBuilder explanation = null, string caravanTicksPerMoveExplanation = null)
            {
                SettingsRef settingsRef = new SettingsRef();

                bool flag = start == end || end == -1;
                if (!flag)
                {
                    if (explanation != null)
                    {
                        explanation.Append(caravanTicksPerMoveExplanation);
                        explanation.AppendLine();
                    }
                    StringBuilder stringBuilder = (explanation == null) ? null : new StringBuilder();
                    float num;
                    if (perceivedStatic && explanation == null)
                    {
                        num = Find.WorldPathGrid.PerceivedMovementDifficultyAt(end);
                    }
                    else
                    {
                        num = WorldPathGrid.CalculatedMovementDifficultyAt(end, perceivedStatic, ticksAbs, stringBuilder);
                    }
                    float roadMovementDifficultyMultiplier = Find.WorldGrid.GetRoadMovementDifficultyMultiplier(start, end, stringBuilder);
                    if (explanation != null)
                    {
                        explanation.AppendLine();
                        explanation.Append("TileMovementDifficulty".Translate() + ":");
                        explanation.AppendLine();
                        explanation.Append(stringBuilder.ToString().Indented("  "));
                        explanation.AppendLine();
                        explanation.Append("  = " + (num * roadMovementDifficultyMultiplier).ToString("0.##"));
                    }
                    int num2 = (int)((float)caravanTicksPerMove * num * roadMovementDifficultyMultiplier);
                    num2 = Mathf.Clamp(num2, 1, 30000);
                    if (settingsRef.speedMultiplier != 1 && explanation != null)
                    {
                        explanation.AppendLine();
                        explanation.AppendLine();
                        explanation.Append("CO_GlobalMultiplierDesc".Translate() + ":");
                        explanation.AppendLine();
                        explanation.Append("  = x" + (settingsRef.speedMultiplier).ToString("0.#"));
                    }
                    int num3 = (int)((float)(num2 / settingsRef.speedMultiplier));
                    if (explanation != null)
                    {
                        explanation.AppendLine();
                        explanation.AppendLine();
                        explanation.Append("FinalCaravanMovementSpeed".Translate() + ":");
                        int num4 = Mathf.CeilToInt((float)num3 / 1f);
                        explanation.AppendLine();
                        explanation.Append(string.Concat(new string[]
                        {
                            "  (",
                            (60000f / (float)caravanTicksPerMove).ToString("0.#"),
                            " / ",
                            (num * roadMovementDifficultyMultiplier).ToString("0.#"),
                            ") * ",
                            settingsRef.speedMultiplier.ToString("0.#"),
                            " = ",
                            (60000f / (float)num4).ToString("0.#"),
                            " ",
                            "TilesPerDay".Translate()
                        }));
                    }
                    __result = num3;
                }
                else
                {
                    return true;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(WorldGrid), "GetRoadMovementDifficultyMultiplier", null)]
        public class GetRoadMovementMultiplierFast_Prefix_Patch
        {
            private static void Postfix(WorldGrid __instance, ref int fromTile, ref int toTile, ref float __result, StringBuilder explanation = null)
            {
                List<Tile.RoadLink> roads = __instance.tiles[fromTile].Roads;
                if (roads != null)
                {
                    SettingsRef settingsRef = new SettingsRef();
                    float multiplier = 0.5f;
                    for (int i = 0; i < roads.Count; i++)
                    {
                        if (roads[i].neighbor == toTile)
                        {
                            if (roads[i].road.defName == "DirtPath" || roads[i].road.defName == "DirtPathBuilt")
                            {
                                __result = settingsRef.dirtPath;
                            }
                            if (roads[i].road.defName == "DirtRoad" || roads[i].road.defName == "DirtRoadBuilt")
                            {
                                __result = settingsRef.dirtRoad;
                            }
                            if (roads[i].road.defName == "StoneRoad" || roads[i].road.defName == "StoneRoadBuilt")
                            {
                                __result = settingsRef.stoneRoad;
                            }
                            if (roads[i].road.defName == "AncientAsphaltRoad" || roads[i].road.defName == "AsphaltRoad")
                            {
                                __result = settingsRef.asphaltRoad;
                            }
                            if (roads[i].road.defName == "AncientAsphaltHighway")
                            {
                                __result = settingsRef.asphaltHighway;
                            }
                            if (roads[i].road.defName == "GlitterRoad")
                            {
                                __result = settingsRef.glitterRoad;
                            }
                            multiplier = __result;
                        }
                    }
                    if (explanation != null && multiplier != 0.5f)
                    {
                        if (explanation.Length > 0)
                        {
                            explanation.AppendLine();
                        }
                        explanation.Append("CO_AdjustedTo".Translate(multiplier.ToStringPercent()));
                    }
                }
            }
        }
    }
}





