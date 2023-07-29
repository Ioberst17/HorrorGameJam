using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SimpleSerializableDictionary<TKey, TValue>
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    public Dictionary<TKey, TValue> Dictionary => dictionary;

    public TValue this[TKey key]
    {
        get { return GetValue(key); }
        set { AddOrUpdate(key, value); }
    }

    public void UpdateDictionary()
    {
        dictionary.Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError("Number of keys does not match number of values.");
            return;
        }

        for (int i = 0; i < keys.Count; i++)
        {
            TKey key = keys[i];
            TValue value = values[i];

            if (key == null)
            {
                Debug.LogError("Found null key at index: " + i);
                continue;
            }

            if (dictionary.ContainsKey(key))
            {
                Debug.LogError("Duplicate key found: " + key.ToString());
                continue;
            }

            dictionary[key] = value;
        }
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return dictionary.TryGetValue(key, out value);
    }

    public void Add(TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
        {
            Debug.LogError("Duplicate key found: " + key.ToString());
            return;
        }

        dictionary.Add(key, value);
        keys.Add(key);
        values.Add(value);
    }

    public void AddOrUpdate(TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
            int index = keys.IndexOf(key);
            values[index] = value;
        }
        else
        {
            Add(key, value);
        }
    }

    public bool Remove(TKey key)
    {
        if (dictionary.Remove(key))
        {
            int index = keys.IndexOf(key);
            keys.RemoveAt(index);
            values.RemoveAt(index);
            return true;
        }

        return false;
    }

    public void Clear()
    {
        dictionary.Clear();
        keys.Clear();
        values.Clear();
    }

    public bool TryGetKey(TValue value, out TKey key)
    {
        foreach (var pair in dictionary)
        {
            if (EqualityComparer<TValue>.Default.Equals(pair.Value, value))
            {
                key = pair.Key;
                return true;
            }
        }

        key = default(TKey);
        return false;
    }

    public List<TKey> GetKeys()
    {
        return keys;
    }

    public TValue GetValue(TKey key)
    {
        TValue value;
        dictionary.TryGetValue(key, out value);
        return value;
    }
}