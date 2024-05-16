using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance { get; private set; }

    protected abstract void Awake();

    protected bool InitializeSingleton(T singletonInstance)
    {
        //must do a bool null check instead of an ordinary null check to avoid off behavoir with destroying MonoBehaviours
        if(!instance)
        {
            instance = singletonInstance;
            return true;
        }
        Destroy(this);
        return false;
    }
}
