using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class WeightedRange
{
    #region Nested Structures

    [System.Serializable]
    private struct WeightPercentage
    {
        [SerializeField]
        private int value;
        [SerializeField, Range(0f, 1f), LabelText("@percentage + \"%\"")]
        private float percentage;

        public WeightPercentage(int value, float percentage)
        {
            this.value = value;
            this.percentage = percentage;
        }
    }

    #endregion

    [SerializeField,
        Tooltip("Defines the min and max boundaries of the range")]
    private MinMaxIntPie _minMaxRange;
    public MinMaxIntPie minMaxRange => _minMaxRange;

    [SerializeField,
        Tooltip("The curve that represents the weight across the range.\nX-axis is the full range from min to max\nY-axis is the weight of a range value")]
    private AnimationCurve weightCurve;

    [FoldoutGroup("Debugging"), SerializeField]
#pragma warning disable 0414
    private bool showPercentages = false;
#pragma warning restore 0414
    [FoldoutGroup("Debugging"), SerializeField, ShowIf(nameof(showPercentages)), ReadOnly]
    private WeightPercentage[] weightPercentages;

    private WeightNode<int>[] weightNodes;
    private float totalChance;

    [Button]
    public void Init()
    {
        if(weightCurve == null)
        {
            Debug.LogError("Missing weight curve!");
            return;
        }

        SetupWeightNodes();
        totalChance = ChanceGenerator.GetTotalChance(weightNodes);
        CalculateWeightPercentages();

        #region Local Methods

        void SetupWeightNodes()
        {
            weightNodes = new WeightNode<int>[minMaxRange.Count()];
            int curr = minMaxRange.Min;
            for(int i = 0; i < weightNodes.Length; i++)
            {
                float perc = minMaxRange.InverseLerp(curr);
                float weight = weightCurve.Evaluate(perc);
                weightNodes[i] = new WeightNode<int>(weight, curr);
                curr++;
            }
        }

        void CalculateWeightPercentages()
        {
            weightPercentages = new WeightPercentage[weightNodes.Length];
            for(int i = 0; i < weightPercentages.Length; i++)
            {
                WeightNode<int> node = weightNodes[i];
                float perc = node.GetChance() / totalChance;
                weightPercentages[i] = new WeightPercentage(node.obj, perc);
            }
        }

        #endregion
    }

    public int Pick(string randID
#if DEBUG_RANDOM_GENERATOR
    , string source    
#endif
    )
    {
        return ChanceGenerator.Pick_RandomUtility<WeightNode<int>>(weightNodes, randID,
#if DEBUG_RANDOM_GENERATOR
            source,
# endif
            totalChance:totalChance).obj;
    }
}
