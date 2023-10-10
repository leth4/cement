using UnityEngine;

public abstract class SingletonPersistent<T> : MonoBehaviour where T : SingletonPersistent<T>
{
    public static T Instance;

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this as T;
            DontDestroyOnLoad(this);
        }
    }
}