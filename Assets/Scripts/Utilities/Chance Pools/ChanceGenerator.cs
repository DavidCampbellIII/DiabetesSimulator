using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceGenerator
{
    public static T Pick<T>(IEnumerable<IChanceable> chanceables, 
        float totalChance=-1f, float noSpawnChance=0f, ChanceModifier[] modifiers = null) where T : IChanceable
    {
        if (chanceables == null || !chanceables.Any())
        {
            //Debug.LogError("==CHANCE PICKER ERROR== Cannot pick from an empty array!");
            return default;
        }

        //calculate the totalChance if a cached totalChance was not sent in
        if(totalChance == -1f)
        {
            float modifiersChanceAdjustment = modifiers?.Sum(x => x.chanceAdjustment) ?? 0f;
            totalChance = GetTotalChance(chanceables, noSpawnChance + modifiersChanceAdjustment);
        }

        float selectedNum = Random.Range(0f, totalChance);
        float runningTotal = 0f;
        foreach(IChanceable chanceable in chanceables)
        {
            if(chanceable == null)
            {
                continue;
            }

            float modifierChanceAdjustment = modifiers?.FirstOrDefault(x => x.chanceable == chanceable)?.chanceAdjustment ?? 0f;
            runningTotal += chanceable.GetChance() + modifierChanceAdjustment;
            if(selectedNum < runningTotal)
            {
                return (T)chanceable;
            }
        }

        return default;
    }

    public static T Pick_RandomUtility<T>(IEnumerable<IChanceable> chanceables, string randID,
#if DEBUG_RANDOM_GENERATOR
        string source, 
#endif
        float totalChance = -1f, float noSpawnChance = 0f, ChanceModifier[] modifiers = null) where T : IChanceable
    {
        if (chanceables == null || !chanceables.Any())
        {
            //Debug.LogError("==CHANCE PICKER ERROR== Cannot pick from an empty array!");
            return default;
        }

        //calculate the totalChance if a cached totalChance was not sent in
        if (totalChance == -1f)
        {
            float modifiersChanceAdjustment = modifiers?.Sum(x => x.chanceAdjustment) ?? 0f;
            totalChance = GetTotalChance(chanceables, noSpawnChance + modifiersChanceAdjustment);
        }

        float selectedNum = RandomUtility.Range(0f, totalChance, randID
#if DEBUG_RANDOM_GENERATOR
                    , source
#endif
                    );
        float runningTotal = 0f;
        foreach (IChanceable chanceable in chanceables)
        {
            if (chanceable == null)
            {
                continue;
            }

            float modifierChanceAdjustment = modifiers?.FirstOrDefault(x => x.chanceable == chanceable)?.chanceAdjustment ?? 0f;
            runningTotal += chanceable.GetChance() + modifierChanceAdjustment;
            if (selectedNum < runningTotal)
            {
                return (T)chanceable;
            }
        }

        return default;
    }

    /// <summary>
    /// Calculate the total chance of a set of chanceables, including the option of adding the chance that nothing will be picked.  
    /// Good for caching chances that don't need to constantly be calculated.
    /// </summary>
    /// <param name="chanceables">Chanceable to generate the total chance from</param>
    /// <param name="noSpawnChance">Extra chance added to give the possibility that nothing from this group of chanceables is picked</param>
    /// <returns>Total chance calculated from all chanceables plus the chance nothing will spawn</returns>
    public static float GetTotalChance(IEnumerable<IChanceable> chanceables, float noSpawnChance=0f)
    {
        if (chanceables == null || !chanceables.Any())
        {
            return 0f;
        }

        float totalChance = noSpawnChance;
        foreach(IChanceable chanceable in chanceables)
        {
            if(chanceable != null)
            {
                totalChance += chanceable.GetChance();
            }
        }
        return totalChance;
    }
}
