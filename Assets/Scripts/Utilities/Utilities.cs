using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyBox;

public static class Utilities
{
	#region Gameplay

	public static void ToggleCursorLock(bool status)
	{
		Cursor.visible = !status;
		Cursor.lockState = status ? CursorLockMode.Locked : CursorLockMode.None;
	}

	#endregion

	#region Arrays

	/// <summary>
	/// Concats one array onto the back of another
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="arr1"></param>
	/// <param name="arr2"></param>
	/// <returns></returns>
	public static T[] ConcatArrays<T>(T[] arr1, T[] arr2)
	{
		T[] result = new T[arr1.Length + arr2.Length];
		int index = 0;
		for (int i = 0; i < arr1.Length; i++)
		{
			result[index++] = arr1[i];
		}

		for (int i = 0; i < arr2.Length; i++)
		{
			result[index++] = arr2[i];
		}

		return result;
	}

	#endregion

	#region Vector3s

	public static Vector3 RandomRange(Vector3 min, Vector3 max)
	{
		return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
	}

    public static Vector3 RandomRange_RandomUtility(Vector3 min, Vector3 max, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
    )
    {
        float x = RandomUtility.Range(min.x, max.x, id
#if DEBUG_RANDOM_GENERATOR
        , source
#endif
        );

        float y = RandomUtility.Range(min.y, max.y, id
#if DEBUG_RANDOM_GENERATOR
        , source
#endif
        );

        float z = RandomUtility.Range(min.z, max.z, id
#if DEBUG_RANDOM_GENERATOR
        , source
#endif
        );

        return new Vector3(x, y, z);
    }

    public static float FastDistance(Vector3 p1, Vector3 p2)
    {
        //no need to sqrt because we are not needing the exact distance, just need to know which is the biggest
        float pr1 = p1.x - p2.x;
        float pr2 = p1.z - p2.z;
        float dist = pr1 * pr1 + pr2 * pr2;
        return Mathf.Abs(dist);
    }

	public static Vector3 LerpEase(Vector3 a, Vector3 b, float t, EaseType easeType, int intensity = 1)
	{
		t = Mathf.Clamp01(t);
		return new()
		{
			x = LerpEaseUnclamped(a.x, b.x, t, easeType, intensity),
			y = LerpEaseUnclamped(a.y, b.y, t, easeType, intensity),
			z = LerpEaseUnclamped(a.z, b.z, t, easeType, intensity)
		};
	}

	#endregion

	#region Vector2s

	public static Vector2 RandomRange(Vector2 min, Vector2 max)
	{
		return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
	}

	#endregion

	#region Quaternions

	/// <summary>
	/// Gets a random rotation (0-359.9999) in the specified Dimensions
	/// </summary>
	/// <param name="dimensions">Each dimension we want a random rotation in</param>
	/// <returns>Random rotation Quaternion</returns>
	public static Quaternion RandomRotation(params Dimension[] dimensions)
	{
		float[] dims = new float[3];

		foreach (Dimension dim in dimensions)
		{
			dims[(int)dim] = Random.Range(0f, 359.9999f);
		}

		return Quaternion.Euler(new Vector3(dims[0], dims[1], dims[2]));
	}

	/// <summary>
	/// Gets a random rotation (0-359.9999) in the specified Dimensions (uses RandomUtility instead of UnityEngine.Random)
	/// </summary>
	/// <param name="id">ID of the random generator to be used</param>
	/// <param name="dimensions">Each dimension we want a random rotation in</param>
	/// <returns>Random rotation Quaternion</returns>
	public static Quaternion RandomRotation_RandomUtility(string id,
#if DEBUG_RANDOM_GENERATOR
        string source,
#endif
				params Dimension[] dimensions)
	{
		float[] dims = new float[3];

		foreach (Dimension dim in dimensions)
		{
			dims[(int)dim] = RandomUtility.Range(0f, 359.9999f, id
#if DEBUG_RANDOM_GENERATOR
                , source
#endif
								);
		}

		return Quaternion.Euler(new Vector3(dims[0], dims[1], dims[2]));
	}

	#endregion

	#region Colors

	public static Color LerpEase(Color a, Color b, float t, EaseType easeType, int intensity = 1)
	{
		t = Mathf.Clamp01(t);
		return new()
		{
			r = LerpEaseUnclamped(a.r, b.r, t, easeType, intensity),
			g = LerpEaseUnclamped(a.g, b.g, t, easeType, intensity),
			b = LerpEaseUnclamped(a.b, b.b, t, easeType, intensity),
			a = LerpEaseUnclamped(a.a, b.a, t, easeType, intensity)
		};
	}

	public static Color RandomColor(float alpha=1f)
    {
        return new Color(Random.value, Random.value, Random.value, alpha);
    }

	#endregion

    #region Bounds

    public static bool IntersectsAny(IEnumerable<Bounds> existing, Bounds toCheck)
    {
        foreach(Bounds other in existing)
        {
            if(other.Intersects(toCheck))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IntersectsNone(IEnumerable<Bounds> existing, Bounds toCheck)
    {
        return !IntersectsAny(existing, toCheck);
    }

    #endregion

    #region Meshes

	/// <summary>
	/// Returns a random point on the given mesh, using the mesh's vertices.  Recommended to use shared mesh from mesh filter unless you know exactly what you are doing otherwise
	/// </summary>
	/// <param name="mesh">Mesh that we want to grab a random point off of</param>
	/// <param name="localTransform">Transform that is used to convert mesh point from local to world space</param>
	/// <returns>Point on the given mesh</returns>
	public static Vector3 GetRandomPointOnMesh(Mesh mesh, Transform localTransform, out Vector3 normal)
	{
		Vector3[] meshPoints = mesh.vertices;
		//get first index of randomly selected triangle triangle
		int triStart = Random.Range(0, meshPoints.Length / 3) * 3;

		//get normal
		Vector3[] meshNormals = mesh.normals;
		Vector3 p1Normal = meshNormals[triStart];
		Vector3 p2Normal = meshNormals[triStart + 1];
		Vector3 p3Normal = meshNormals[triStart + 2];
		normal = (p1Normal + p2Normal + p3Normal) / 3f;

		//get random point inside the selected triangle
		Vector3 p1 = meshPoints[triStart];
		Vector3 p2 = meshPoints[triStart + 1];
		Vector3 p3 = meshPoints[triStart + 2];

		float a = Random.Range(0f, 1f);
		float b = Random.Range(0f, 1f);
		if (a + b >= 1)
		{
			a = 1 - a;
			b = 1 - b;
		}

		Vector3 pointOnMesh = p1 + a * (p2 - p1) + b * (p3 - p1);
		//convert to worldspace
		return localTransform.TransformPoint(pointOnMesh);
	}

	#endregion

	#region Editor

	/// <summary>
	/// Makes sure that scriptable objects that are modified during runtime are properly serialized and saved upon exiting Unity.
	/// </summary>
	/// <param name="obj">Scriptable object instance to save</param>
	public static void SaveRuntimeModifiedScriptableObject(ScriptableObject obj)
	{
#if UNITY_EDITOR
		EditorUtility.SetDirty(obj);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
#endif
	}

	#endregion

	#region Easing

	public static float LerpEase(float a, float b, float t, EaseType easeType, int intensity = 1)
	{
		return LerpEaseUnclamped(a, b, Mathf.Clamp01(t), easeType, intensity);
	}

	public static float LerpEaseUnclamped(float a, float b, float t, EaseType easeType, int intensity = 1)
	{
		return easeType switch
		{
			EaseType.LINEAR => Mathf.LerpUnclamped(a, b, t),
			EaseType.IN => LerpEaseIn(a, b, t, intensity),
			EaseType.OUT => LerpEaseOut(a, b, t, intensity),
			_ => a
		};
	}

	private static float LerpEaseOut(float a, float b, float t, int intensity = 1)
	{
		float finalT = 1 - t;
		for (int i = 0; i < intensity; i++)
		{
			finalT *= 1 - t;
		}

		return a + ((b - a) * (1 - finalT));
	}

	private static float LerpEaseIn(float a, float b, float t, int intensity = 1)
	{
		float finalT = t;
		for (int i = 0; i < intensity; i++)
		{
			finalT *= t;
		}

		return a + ((b - a) * finalT);
	}

	#endregion

	#region Other

	public static Vector2 RandomPosInRadius(float minDistance, float maxDistance)
	{
		return RandomPosInRadius(new MinMaxFloat(minDistance, maxDistance));
	}

	public static Vector2 RandomPosInRadius(MinMaxFloat minMaxDistance)
	{
		Vector2 dir = Vector2.zero;
		while (dir == Vector2.zero)
		{
			dir = Random.insideUnitCircle;
		}

		return dir.normalized * minMaxDistance.RandomInRange();
	}

	/// <summary>
	/// Returns true roughly 50% of the time
	/// </summary>
	/// <returns></returns>
	public static bool FlipCoin()
	{
		return Random.Range(0, 2) == 0;
	}

	public static int PossibleNegative()
	{
		return FlipCoin() ? 1 : -1;
	}

	#endregion
}
