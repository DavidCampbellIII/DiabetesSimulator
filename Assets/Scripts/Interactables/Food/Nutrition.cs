using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[System.Serializable]
public class Nutrition
{
    [SerializeField, PositiveValueOnly]
    private float _calories;
    public float calories => _calories;
    [SerializeField, PositiveValueOnly]
    private float _fat;
    public float fat => _fat;
    [SerializeField, PositiveValueOnly]
    private float _carbs;
    public float carbs => _carbs;
    [SerializeField, PositiveValueOnly]
    private float _fiber;
    public float fiber => _fiber;
    [SerializeField, PositiveValueOnly]
    private float _protein;
    public float protein => _protein;
    
    [SerializeField, PositiveValueOnly]
    private float _glycemicIndex;
    public float glycemicIndex => _glycemicIndex;
    
    public float NetCarbs => carbs - fiber;
}
