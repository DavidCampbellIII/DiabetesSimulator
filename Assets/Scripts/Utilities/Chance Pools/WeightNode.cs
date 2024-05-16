using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class WeightNode<T> : IChanceable, ICloneable
{
    [SerializeField, PositiveValueOnly]
    private float weight = 1f;

    [SerializeField, LabelText("@GetObjLabel()")]
    private T _obj;
    public T obj => _obj;

    #region Inspector Validation

    private Type GetObjLabel()
    {
        //if it's an object, try to get the specific type if it's not null
        if (default(T) == null)
        {
            return _obj?.GetType() ?? GetType().GetGenericArguments()[0];
        }
        return typeof(T);
    }

    #endregion

    public WeightNode()
    {}

    public WeightNode(float weight, T obj)
    {
        this.weight = weight;
        _obj = obj;
    }

    public float GetChance()
    {
        return weight;
    }

    public object Clone()
    {
        return new WeightNode<T>(weight, _obj);
    }
}
