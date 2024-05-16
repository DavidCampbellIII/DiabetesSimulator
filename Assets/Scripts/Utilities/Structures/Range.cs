using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Range<T>
{
    [field: SerializeField, UsePropertyName] //allows each field to be read only, except by the inspector
    public T min { get; private set; }
    [field: SerializeField, UsePropertyName]
    public T max { get; private set; }
}
