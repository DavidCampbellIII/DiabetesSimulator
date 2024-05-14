using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using Shapes;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Graph : MonoBehaviour
{
    private const float SECONDS_IN_DAY = 24f * 60f * 60f;
    
    #region Nested Structures
    
    [Serializable]
    private struct GlucoseRange
    {
        [SerializeField]
        private MinMaxFloat _range;
        public MinMaxFloat range => _range;
        
        [SerializeField]
        private Color _normalColor;
        public Color normalColor => _normalColor;
        
        [SerializeField]
        private Color _lowColor;
        public Color lowColor => _lowColor;
        
        [SerializeField]
        private Color _highColor;
        public Color highColor => _highColor;
        
        public bool IsInRange(float value)
        {
            return range.IsInRange(value);
        }
        
        public Color GetColor(float value)
        {
            if (range.IsInRange(value))
            {
                return normalColor;
            }
            return value < range.Min ? lowColor : highColor;
        }
    }
    
    #endregion
    
    [SerializeField]
    private MinMaxFloat minMaxGraphHeight;
    [SerializeField, PositiveValueOnly]
    private float graphXScale = 24f;
    [SerializeField, MustBeAssigned]
    private float graphYScale = 1f;
    [SerializeField]
    private GlucoseRange range;
    
    [FoldoutGroup("SHAPE REFERENCES", expanded:true), SerializeField, MustBeAssigned]
    private Polyline line;
    [FoldoutGroup("SHAPE REFERENCES"), SerializeField, MustBeAssigned]
    private Rectangle lowRangeRect;
    [FoldoutGroup("SHAPE REFERENCES"), SerializeField, MustBeAssigned]
    private Rectangle highRangeRect;
    
    [FoldoutGroup("UI REFERENCES"), SerializeField, MustBeAssigned]
    private TextMeshProUGUI readingText;
    
    private readonly List<BloodGlucoseReading> readings = new List<BloodGlucoseReading>();
    
    private void Start()
    {
        line.points.Clear();
        
        float xPosition = graphXScale / 2f;
        
        lowRangeRect.Color = range.lowColor;
        lowRangeRect.Width = graphXScale;
        lowRangeRect.Height = range.range.Min * graphYScale;
        lowRangeRect.transform.localPosition = new Vector3(xPosition, lowRangeRect.Height / 2f, 0f);
        
        highRangeRect.Color = range.highColor;
        highRangeRect.Width = graphXScale;
        highRangeRect.Height = (minMaxGraphHeight.Max - range.range.Max) * graphYScale;
        highRangeRect.transform.localPosition = new Vector3(xPosition, minMaxGraphHeight.Max * graphYScale - highRangeRect.Height / 2f, 0f);
    }
    
    public void AddReading(BloodGlucoseReading reading)
    {
        readings.Add(reading);
        readingText.text = reading.reading.ToString("N0");
        readingText.color = range.GetColor(reading.reading);
        UpdateGraph();
    }
    
    private void UpdateGraph()
    {
        // Sort readings by time to ensure the graph is drawn correctly
        readings.Sort((x, y) => x.time.CompareTo(y.time));
        
        line.points.Clear();
        for (int i = 0; i < readings.Count; i++)
        {
            float normalizedTime = readings[i].time / SECONDS_IN_DAY;
            float xPosition = normalizedTime * graphXScale;
            float yPosition = Mathf.Clamp(readings[i].reading, minMaxGraphHeight.Min, minMaxGraphHeight.Max) * graphYScale;
            Color color = range.GetColor(readings[i].reading).WithAlphaSetTo(255f);
            line.AddPoint(new Vector3(xPosition, yPosition, 0f), color);
        }
    }
}
