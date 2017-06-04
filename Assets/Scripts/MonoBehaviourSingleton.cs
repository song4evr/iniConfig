using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{

    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>() as T;
                if (instance == null)
                {
                    GameObject GObj = new GameObject(typeof(T).Name);
                    instance = GObj.AddComponent<T>();
                    DontDestroyOnLoad(instance);
                }
            }
            return instance;
        }
    }

    public static bool HasInstance
    {
        get
        {
            return instance != null;
        }

    }

}
