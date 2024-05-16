using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to modify the chance of a single IChanceable on a case-by-case basis.
/// Example: Want to pick from a pool of items, but this one single instance of picking should favor one single item more than usual.
/// </summary>
public class ChanceModifier
{
    public IChanceable chanceable { get; }
    public float chanceAdjustment { get; }

    public ChanceModifier(IChanceable chanceable, float chanceAdjustment)
    {
        this.chanceable = chanceable;
        this.chanceAdjustment = chanceAdjustment;
    }
}
