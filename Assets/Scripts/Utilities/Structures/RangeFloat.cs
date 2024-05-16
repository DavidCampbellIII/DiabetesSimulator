using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is needed as a wrapper class so that Range can be shown in the Inspector on the ScriptableObject
//(Unity does not support the serialization of generics :(  )
[Serializable]
public class RangeFloat : Range<float>
{
}
