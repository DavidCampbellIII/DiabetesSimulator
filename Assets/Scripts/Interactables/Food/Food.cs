using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class Food : Interactable
{
    [SerializeField, PositiveValueOnly]
    private float servings = 1f;
    
    [SerializeField, MustBeAssigned]
    private Nutrition nutrition;
    
    protected override void Interact_Internal()
    {
        BloodGlucoseSimulator.AddCarbs(nutrition.NetCarbs, nutrition.glycemicIndex);
        servings--;
        
        if(servings <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
