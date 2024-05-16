using System.Collections;
using System.Collections.Generic;
using System.Text;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
    #region Arrays

    public static T RandomElement<T>(this T[] arr)
    {
        if (arr.Length == 0)
        {
            Debug.LogError("ArrayEmptyError - Trying to access random element from an empty array!");
        }
        return arr[Random.Range(0, arr.Length)];
    }

    public static T RandomElement_RandomUtility<T>(this T[] arr, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        if (arr.Length == 0)
        {
            Debug.LogError("ArrayEmptyError - Trying to access random element from an empty array!");
        }
        return arr[RandomUtility.Range(0, arr.Length, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            )];
    }

    public static void Shuffle<T>(this T[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            // Pick a random index from 0 to i 
            int j = Random.Range(0, i + 1);

            // Swap arr[i] with the element at random index 
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }

    public static void Shuffle_RandomUtility<T>(this T[] arr, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            // Pick a random index from 0 to i 
            int j = RandomUtility.Range(0, i + 1, id
#if DEBUG_RANDOM_GENERATOR
                , source
#endif
                );

            // Swap arr[i] with the element at random index 
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }

    public static string ToStringPretty<T>(this T[] arr)
    {
        StringBuilder builder = new StringBuilder("[");
        builder.Append(string.Join(", ", arr)).Append("]");
        return builder.ToString();
    }

    #endregion

    #region Lists

    public static T RandomElement<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            Debug.LogError("ListEmptyError - Trying to access random element from an empty list!");
        }
        return list[Random.Range(0, list.Count)];
    }

    public static T RandomElement_RandomUtility<T>(this List<T> list, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        if (list.Count == 0)
        {
            Debug.LogError("ListEmptyError - Trying to access random element from an empty list!");
        }
        return list[RandomUtility.Range(0, list.Count, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            )];
    }

    public static T RemoveRandomElement<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            Debug.LogError("ListEmptyError - Trying to access random element from an empty list!");
        }
        int index = Random.Range(0, list.Count);
        T element = list[index];
        list.RemoveAt(index);
        return element;
    }

    public static T RemoveRandomElement_RandomUtility<T>(this List<T> list, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        if (list.Count == 0)
        {
            Debug.LogError("ListEmptyError - Trying to access random element from an empty list!");
        }
        int index = RandomUtility.Range(0, list.Count, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            );
        T element = list[index];
        list.RemoveAt(index);
        return element;
    }

    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            // Pick a random index from 0 to i 
            int j = Random.Range(0, i + 1);

            // Swap arr[i] with the element at random index 
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static void Shuffle_RandomUtility<T>(this List<T> list, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            // Pick a random index from 0 to i 
            int j = RandomUtility.Range(0, i + 1, id
#if DEBUG_RANDOM_GENERATOR
                , source
#endif
                );

            // Swap arr[i] with the element at random index 
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static List<T> Clone<T>(this List<T> list) where T : System.ICloneable //only possible if object is cloneable
    {
        List<T> result = new List<T>(list.Count);

        for (int i = 0; i < list.Count; i++)
        {
            result.Add((T)list[i].Clone());
        }

        return result;
    }

    public static string ToStringPretty<T>(this List<T> arr)
    {
        StringBuilder builder = new StringBuilder("[");
        builder.Append(string.Join(", ", arr)).Append("]");
        return builder.ToString();
    }

    #endregion

    #region Dictionaries

    public static void AddOrUpdate<K,V>(this Dictionary<K, V> dictionary, K key, V value)
    {
        if (key == null)
        {
            throw new System.ArgumentNullException();
        }

        if (dictionary.ContainsKey(key))
        {
            // already exists so update
            dictionary[key] = value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }

    #endregion

    #region Floats

    //clamps a float to a maximum
    public static void ClampUpper(this ref float num, float max)
    {
        if (num > max)
        {
            num = max;
        }
    }

    //clamps a float to a minimum
    public static void ClampLower(this ref float num, float min)
    {
        if (num < min)
        {
            num = min;
        }
    }

    //clamps a float between a range
    public static void ClampRange(this ref float num, float min, float max)
    {
        num.ClampLower(min);
        num.ClampUpper(max);
    }

    //clamps a float between a range
    public static void ClampRange(this ref float num, Range<float> range)
    {
        num.ClampRange(range.min, range.max);
    }

    /// <summary>
    /// 50% to negate this number
    /// </summary>
    /// <param name="num"></param>
    public static void PossiblyNegate(this ref float num)
    {
        if (Random.Range(0, 2) == 0)
        {
            num *= -1;
        }
    }

    /// <summary>
    /// 50% to negate this number (uses RandomUtility instead of UnityEngine.Random)
    /// </summary>
    public static void PossiblyNegate_RandomUtility(this ref float num, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        if (RandomUtility.Range(0, 2, id
#if DEBUG_RANDOM_GENERATOR
                , source
#endif
                ) == 0)
        {
            num *= -1;
        }
    }

    /// <summary>
    /// Checks if 2 floats are within a certain threshold of one another, inclusive
    /// </summary>
    /// <param name="num">This number (extension method for floats)</param>
    /// <param name="target">The number we are checking</param>
    /// <param name="threshold">The threshold we are checking to see if these two numbers are within</param>
    /// <returns>Returns if the 2 numbers are within "threshold" distance of one another</returns>
    public static bool Within(this float num, float target, float threshold)
    {
        return Mathf.Abs(num - target) <= threshold;
    }

    /// <summary>
    /// Treats this float as a coin flip with a "this float"% chance of happening.  Values over 1 default to 100% chance.
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static bool Chance(this float num)
    {
        return Random.Range(0f, 1.0000001f) < num; //slightly more than 1, just in case the num is 1 exactly
    }

    /// <summary>
    /// Treats this float as a coin flip with a "this float"% chance of happening.  Values over 1 default to 100% chance. (uses RandomUtility instead of UnityEngine.Random)
    /// </summary>
    /// <param name="id">ID of the random generator to be used</param>
    public static bool Chance_RandomUtility(this float num, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        return RandomUtility.Range(0f, 1.0000001f, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            ) < num; //slightly more than 1, just in case the num is 1 exactly
    }

    /// <summary>
    /// Returns a random value between [-num, num]
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static float RandomSwing(this float num)
    {
        return Random.Range(-num, num);
    }

    /// <summary>
    /// Returns a random value between [-num, num] (uses RandomUtility instead of UnityEngine.Random)
    /// </summary>
    /// <param name="id">ID of the random generator to be used</param>
    public static float RandomSwing_RandomUtility(this float num, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        return RandomUtility.Range(-num, num, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            );
    }

    #endregion

    #region Ints

    //clamps a int to a maximum
    public static void ClampUpper(this ref int num, int max)
    {
        if (num > max)
        {
            num = max;
        }
    }

    //clamps a int to a minimum
    public static void ClampLower(this ref int num, int min)
    {
        if (num < min)
        {
            num = min;
        }
    }

    //clamps a int between a range
    public static void ClampRange(this ref int num, int min, int max)
    {
        num.ClampLower(min);
        num.ClampUpper(max);
    }

    //clamps a float between a range
    public static void ClampRange(this ref float num, Range<int> range)
    {
        num.ClampRange(range.min, range.max);
    }

    /// <summary>
    /// Returns a value between [num, num], unless includeUpperBound is false.  Then it returns [num, num).
    /// </summary>
    /// <param name="num"></param>
    /// <param name="includeUpperBound">Num should be included in the upper bound.  Otherwise, it is num - 1</param>
    /// <returns></returns>
    public static float RandomSwing(this int num, bool includeUpperBound=true)
    {
        return Random.Range(-num, includeUpperBound ? num + 1 : num);
    }

    /// <summary>
    /// Returns a value between [num, num], unless includeUpperBound is false.  Then it returns [num, num). (uses RandomUtility instead of UnityEngine.Random)
    /// </summary>
    /// <param name="id">ID of the random generator to be used</param>
    /// <param name="includeUpperBound">Num should be included in the upper bound.  Otherwise, it is num - 1</param>
    public static float RandomSwing_RandomUtility(this int num, string id,
#if DEBUG_RANDOM_GENERATOR
        string source,
#endif
        bool includeUpperBound = true)
    {
        return RandomUtility.Range(-num, includeUpperBound ? num + 1 : num, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            );
    }

    #endregion

    #region Vector2s

    public static Vector2 ToSin(this Vector2 v)
    {
        return new Vector2(Mathf.Sin(v.x), Mathf.Sin(v.y));
    }

    #endregion

    #region Vector3s

    public static Vector2 ToVector2XZ(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 ToSin(this Vector3 v)
    {
        return new Vector3(Mathf.Sin(v.x), Mathf.Sin(v.y), Mathf.Sin(v.z));
    }

    public static Vector3 Abs(this Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    #endregion

    #region Color

    public static void SetColorAlpha(this Graphic origin, float newAlpha)
    {
        Color c = origin.color;
        origin.color = new Color(c.r, c.g, c.b, newAlpha);
    }

    #endregion

    #region GameObjects

    public static void SetLayerRecursively(this GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach(Transform t in obj.transform)
        {
            t.gameObject.SetLayerRecursively(layer);
        }
    }

    #endregion

    #region Transforms

    /// <summary>
    /// Transforms local Bounds to world Bounds using the position, rotation, and scale of this transform
    /// </summary>
    /// <param name="localBounds">Local bounds to transform to world space</param>
    public static Bounds TransformBounds(this Transform t, Bounds localBounds)
    {
        Vector3 center = t.TransformPoint(localBounds.center);

        Vector3 extents = localBounds.extents;
        Vector3 xAxis = t.TransformVector(extents.x, 0f, 0f);
        Vector3 yAxis = t.TransformVector(0f, extents.y, 0f);
        Vector3 zAxis = t.TransformVector(0f, 0f, extents.z);

        extents.x = Mathf.Abs(xAxis.x) + Mathf.Abs(yAxis.x) + Mathf.Abs(zAxis.x);
        extents.y = Mathf.Abs(xAxis.y) + Mathf.Abs(yAxis.y) + Mathf.Abs(zAxis.y);
        extents.z = Mathf.Abs(xAxis.z) + Mathf.Abs(yAxis.z) + Mathf.Abs(zAxis.z);

        return new Bounds { center = center, extents = extents };
    }


    public static void DestroyChildren(this Transform trans)
    {
        foreach (Transform child in trans)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    #endregion

    #region Renderer

    public static bool IsVisibleFrom(this Renderer rend, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, rend.bounds);
    }

    #endregion

    #region AudioSources

    public static void SetClipAndPlay(this AudioSource source, AudioClip clip, bool shouldLoop=false)
    {
        source.clip = clip;
        source.loop = shouldLoop;
        source.Play();
    }

    #endregion

    #region Bounds

    public static Bounds OffsetCenter(this Bounds bounds, Vector3 offset)
    {
        return new Bounds(bounds.center + offset, bounds.size);
    }

    #endregion

    #region Rects

    /// <summary>
    /// Picks a random point within this Rect
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Vector2 RandomPointInside(this Rect rect)
    {
        float x = Random.Range(rect.xMin, rect.xMax);
        float y = Random.Range(rect.yMin, rect.yMax);
        return new Vector2(x, y);
    }

    /// <summary>
    /// Picks a random point within this Rect (uses RandomUtility instead of UnityEngine.Random)
    /// </summary>
    /// <param name="id">ID of the random generator to be used</param>
    public static Vector2 RandomPointInside_RandomUtility(this Rect rect, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        float x = RandomUtility.Range(rect.xMin, rect.xMax, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            );
        float y = RandomUtility.Range(rect.yMin, rect.yMax, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            );
        return new Vector2(x, y);
    }

    #endregion

    #region MinMax

    public static float RandomInRange_RandomUtility(this MinMaxFloat minMax, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        return RandomUtility.Range(minMax.Min, minMax.Max, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            );
    }

    public static int RandomInRange_RandomUtility(this MinMaxInt minMax, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        return RandomUtility.Range(minMax.Min, minMax.Max, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            );
    }

    public static int RandomInRangeInclusive_RandomUtility(this MinMaxInt minMax, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        return RandomUtility.RangeInclusive(minMax.Min, minMax.Max, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            );
    }

    /// <summary>
    /// Total number of values between the Min and Max (including 0).<br />
    /// Example: MinMax(-5, 5) has a count of 11, MinMax(0, 3) has a count of 4
    /// </summary>
    public static int Count(this MinMaxInt minMax)
    {
        return minMax.Length() + 1;
    }

    public static float InverseLerp(this MinMaxInt minMax, int value)
    {
        if(value <= minMax.Min)
        {
            return 0f;
        }

        return value >= minMax.Max ? 1f : Mathf.InverseLerp(minMax.Min, minMax.Max, value);
    }

    #endregion

    #region Other

    public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
    {
        if (!scriptableObject)
        {
            Debug.LogError($"ScriptableObject was null. Returning default {typeof(T)} object.");
            return (T)ScriptableObject.CreateInstance(typeof(T));
        }

        T instance = Object.Instantiate(scriptableObject);
        instance.name = scriptableObject.name; // remove (Clone) from name
        return instance;
    }

    private static readonly AnimationCurve fadeInCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    private static readonly AnimationCurve fadeOutCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
    public static IEnumerator FadeGraphic(this Graphic g, float time, FadeDirection direction)
    {
        switch (direction)
        {
            case FadeDirection.In:
                return FadeGraphic(g, time, fadeInCurve);
            case FadeDirection.Out:
            default:
                return FadeGraphic(g, time, fadeOutCurve);
        }
    }


    public static IEnumerator FadeGraphic(this Graphic g, float time, AnimationCurve curve)
    {
        float t = 0;
        while (t < time)
        {
            g.SetColorAlpha(curve.Evaluate(t / time));

            t += Time.deltaTime;
            yield return null;
        }
    }

    #endregion
}
