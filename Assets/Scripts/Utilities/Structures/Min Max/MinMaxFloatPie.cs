using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MinMaxFloatPie
{
    public float Min;
    public float Max;

    public MinMaxFloatPie(float min, float max)
    {
        Min = min; 
        Max = max;
    }

    public bool IsInRange(float value)
    {
        return value >= Min && value <= Max;
    }

    public float Clamp(float value)
    {
        return Mathf.Clamp(value, Min, Max);
    }

    public float Length()
    {
        return Max - Min;
    }

    public float MidPoint()
    {
        return Min + Length() / 2f;
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

    public float RandomInRange()
    {
        return Random.Range(Min, Max);
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
}
