using System.Collections.Generic;
using UnityEngine;
using System;



public class Cache : MonoBehaviour

{
    private Dictionary<string, object> cache = new Dictionary<string, object>();
    private List<List<string>> cacheLifeTime = new List<List<string>>() { null };

    private ConfigFile configFile;


    void Awake() {
        configFile = MonoBehaviourSingleton<ConfigFile>.Instance;

        Dictionary<string, string> configs = configFile.GetConfigDict();
        Dictionary<string, string>.Enumerator confEnum = configs.GetEnumerator();

        while (confEnum.MoveNext())
        {
            MonoBehaviourSingleton<Cache>.Instance.SetValue(confEnum.Current.Key, confEnum.Current.Value, 0);
        }

        configs.Clear();

        configs = null;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void RemoveLifeTime(string key)
    {
        cacheLifeTime.ForEach(item => { if (item != null) item.ForEach(str => { if (key.Equals(str)) item.Remove(str); }); });
    }
    public void SetValue(string key, object value, int timeOut = 20)
    {
        if (cache.ContainsKey(key))
        {
            RemoveLifeTime(key);
        }
        if (timeOut > 0)
        {
            if (cacheLifeTime.Count > timeOut)
            {
                if (cacheLifeTime[timeOut] == null)
                {
                    cacheLifeTime[timeOut] = new List<string>() { key };
                }
                else
                {
                    cacheLifeTime[timeOut].Add(key);
                }

            }
            else
            {
                for (int i = cacheLifeTime.Count; i < timeOut; i++)
                {
                    cacheLifeTime.Add(null);
                }
                cacheLifeTime.Add(new List<string>() { key });
            }
        }



        cache[key] = value;
    }

    public T ChangeType<T>(object obj)
    {
        return (T)Convert.ChangeType(obj, typeof(T));
    }

    public object GetValue(string key)
    {
        return cache[key];
    }
    public T GetValue<T>(string key)
    {
        T value = default(T);
        TryGetValue<T>(key, out value);
        return value;
    }

    public List<T> GetValues<T>(string key)
    {
        List<T> values = new List<T>();
        Debug.Log(key);
        string valueStr = GetValue<String>(key);
        if (valueStr!=null)
        {
                    
        foreach (string value in valueStr.Split(','))
            {
                values.Add(ChangeType<T>(value));
            }
        }

        return values;
    }

    public Vector3 GetVector3(string key)
    {
        List<float> values = GetValues<float>(key);
        return new Vector3(values[0], values[1], values[2]);
    }

    public bool TryGetValue(string key, out object value)
    {
        return cache.TryGetValue(key, out value);
    }

    public bool TryGetValue<T>(string key, out T value)
    {
        object result;
        bool isGetValue = cache.TryGetValue(key, out result);
        if (isGetValue)
        {
            value = ChangeType<T>(result);
        }
        else {
            value = default(T);
        }
        return isGetValue;
    }

    public bool ContainsKey(string key)
    {
        return cache.ContainsKey(key);
    }
    public bool Remove(string key)
    {
        RemoveLifeTime(key);
        return cache.Remove(key);
    }


    void OnLevelWasLoaded(int level)
    {
        if (cacheLifeTime[0] != null)
        {
            cacheLifeTime[0].ForEach(key => cache.Remove(key));
            cacheLifeTime[0].Clear();
        }
        cacheLifeTime.RemoveAt(0);
        cacheLifeTime.Add(null);
    }



}

