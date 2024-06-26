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
    [FoldoutGroup("SHAPE REFERENCES"), SerializeField, MustBeAssigned]
    private Line gridLinePrefab;
    
    [FoldoutGroup("UI REFERENCES"), SerializeField, MustBeAssigned]
    private TextMeshProUGUI readingText;
    [FoldoutGroup("UI REFERENCES"), SerializeField, MustBeAssigned]
    private TextMeshProUGUI basalRateText;
    [FoldoutGroup("UI REFERENCES"), SerializeField, MustBeAssigned]
    private TextMeshProUGUI deltaText;
    [FoldoutGroup("UI REFERENCES"), SerializeField, MustBeAssigned]
    private TextMeshProUGUI timeInRangeText;
    [FoldoutGroup("UI REFERENCES"), SerializeField, MustBeAssigned]
    private TextMeshProUGUI insulinOnBoardText;
    [FoldoutGroup("UI REFERENCES"), SerializeField, MustBeAssigned]
    private TextMeshProUGUI sugarOnBoardText;
    
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
        
        CreateHourlyGridLines();
        
        #region Local Methods
        
        void CreateHourlyGridLines()
        {
            for (int i = 0; i <= 24; i++)
            {
                float normalizedTime = i / 24f;
                float xPos = normalizedTime * graphXScale;
                Line gridLine = Instantiate(gridLinePrefab, transform);
                gridLine.Start = new Vector3(xPos, 0f, 0f);
                gridLine.End = new Vector3(xPos, minMaxGraphHeight.Max * graphYScale, 0f);
            }
        }
        
        #endregion
    }
    
    public void AddReading(BloodGlucoseReading reading)
    {
        readings.Add(reading);
        readingText.text = reading.reading.ToString("N0");
        readingText.color = range.GetColor(reading.reading);
        UpdateGraph();
    }
    
    public void UpdateBasalRate(float basalRate)
    {
        basalRateText.text = basalRate.ToString("N2");
    }
    
    public void UpdateStats(float delta, float insulinOnBoard, float sugarOnBoard)
    {
        deltaText.text = delta.ToString("+#;-#;0");
        insulinOnBoardText.text = insulinOnBoard.ToString("N2");
        sugarOnBoardText.text = sugarOnBoard.ToString("N0");
    }
    
    private void UpdateGraph()
    {
        // Sort readings by time to ensure the graph is drawn correctly
        readings.Sort((x, y) => x.time.CompareTo(y.time));
        
        line.points.Clear();
        int numInRange = 0;
        for (int i = 0; i < readings.Count; i++)
        {
            float normalizedTime = readings[i].time / SECONDS_IN_DAY;
            float xPosition = normalizedTime * graphXScale;
            float yPosition = Mathf.Clamp(readings[i].reading, minMaxGraphHeight.Min, minMaxGraphHeight.Max) * graphYScale;
            Color color = range.GetColor(readings[i].reading).WithAlphaSetTo(255f);
            line.AddPoint(new Vector3(xPosition, yPosition, 0f), color);
            
            if (range.IsInRange(readings[i].reading))
            {
                numInRange++;
            }
        }
        
        float percentageInRange = (float)numInRange / readings.Count;
        timeInRangeText.text = $"{percentageInRange:P0}";
    }
}
