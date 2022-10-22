using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using RimWorld;

namespace CaravanOptions.ModCheck
{
    public class Validate
    {
        public static class RoadsOfTheRim
        {
            public static bool IsInitialized()
            {
                bool initialized = false;
                foreach (ModContentPack p in LoadedModManager.RunningMods)
                {
                    if (p.Name == "Roads of the Rim" || p.Name == "Roads of the Rim (Continued)")
                    {
                        initialized = true;
                    }
                }
                return initialized;
            }
        }
    }
}
