using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MinMaxIntPie
{
    public int Min;
    public int Max;

    public MinMaxIntPie(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public bool IsInRange(int value)
    {
        return value >= Min && value <= Max;
    }

    public int Clamp(int value)
    {
        return Mathf.Clamp(value, Min, Max);
    }

    public int Length()
    {
        return Max - Min;
    }

    /// <summary>
    /// Total number of values between the Min and Max (including 0).<br />
    /// Example: MinMax(-5, 5) has a count of 11, MinMax(0, 3) has a count of 4
    /// </summary>
    public int Count()
    {
        return Length() + 1;
    }

    public int MidPoint()
    {
        return Min + Length() / 2;
    }

    public float Lerp(float value)
    {
        return Mathf.Lerp(Min, Max, value);
    }

    public float LerpUnclamped(float value)
    {
        return Mathf.LerpUnclamped(Min, Max, value);
    }

    public float InverseLerp(int value)
    {
        if (value <= Min)
        {
            return 0f;
        }

        return value >= Max ? 1f : Mathf.InverseLerp(Min, Max, value);
    }

    public int RandomInRange()
    {
        return Random.Range(Min, Max);
    }

    public int RandomInRangeInclusive()
    {
        return Random.Range(Min, Max + 1);
    }

    public float RandomInRange_RandomUtility(string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        return RandomUtility.Range(Min, Max, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            );
    }

    public int RandomInRangeInclusive_RandomUtility(string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        return RandomUtility.RangeInclusive(Min, Max, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            );
    }
}
