using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeableFloat
{
    [SerializeField]
    private float _value;
    public float value { get => _value; set => _value = value; }

    [SerializeField]
    private float _originValue;
    public float originValue { get => _originValue; set => _originValue = value; }

    public UpgradeableFloat(float initalVal=1f) : this(initalVal, initalVal)
    {
        //purposely empty
    }

    protected UpgradeableFloat(float initialVal, float originVal)
    {
        value = initialVal;
        originValue = originVal;
    }

    /// <summary>
    /// Apply some multiplier to this float
    /// </summary>
    /// <param name="multiplier">Multiplier to apply</param>
    /// <param name="applyToCurrentValue">Should the multiplier be applied to the current value, or to the origin value?</param>
    /// <param name="clampRange">The min and max range that the value is allowed to be</param>
    public void Upgrade(float multiplier, bool applyToCurrentValue=false, RangeFloat clampRange=null)
    {
        if(applyToCurrentValue)
        {
            value *= multiplier;
        }
        else
        {
            value = originValue * multiplier;
        }

        ClampRange(clampRange);
    }

    public void ResetToOrigin()
    {
        value = originValue;
    }

    public static implicit operator float(UpgradeableFloat f)
    {
        return f.value;
    }

    public static implicit operator UpgradeableFloat(float value)
    {
        return new UpgradeableFloat(value, value);
    }

    public void ClampRange(RangeFloat clampRange)
    {
        if (clampRange == null) return;

        _value.ClampRange(clampRange);
    }
}
