using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    static T _instance = null;
    public static T instance
    {
        get
        {
            if (!_instance)
            {
#if UNITY_EDITOR
                T[] otherInstances = GetAllExistingT();
                if (otherInstances.Length == 0)
                {
                    string errorMsg = "==MISSING SINGLETON ERROR== No singleton scriptable objects of type " + typeof(T) + " exist!";
                    Debug.LogError(errorMsg);
                    return null;
                }
                else if (otherInstances.Length > 1)
                {
                    string errorMsg = "==MULTIPLE SINGLETON ERROR== Multiple instances of " + typeof(T) + " exist!  Ensure only one instance ever exists!";
                    Debug.LogError(errorMsg);
                }

                _instance = otherInstances[0];
                _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
#else
                string errorMsg = "==MISSING SINGLETON ERROR== No singleton scriptable objects of type " + typeof(T) + " exist!";
                Debug.LogError(errorMsg);
                return null;
#endif
            }
            return _instance;
        }
    }

#if UNITY_EDITOR
    private void Awake()
    {
        T[] otherInstances = GetAllExistingT();
        if (otherInstances.Length > 1)
        {
            string errorMsg = "==MULTIPLE SINGLETON ERROR== Multiple instances of " + typeof(T) + " exist!  Ensure only one instance ever exists!";
            Debug.LogError(errorMsg);
        }
    }

    private static T[] GetAllExistingT()
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        T[] others = new T[guids.Length];

        for(int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            others[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }
        return others;
    }
#endif
}
