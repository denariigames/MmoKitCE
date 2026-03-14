using System;
using System.Collections.Generic;
using UnityEngine;

public class ComponentCollector
{
    private static readonly Dictionary<Type, List<object>> cache = new Dictionary<Type, List<object>>();
    public static void Add<T>(T comp) where T : Component
    {
        Type type = comp.GetType();
        if (!cache.ContainsKey(type))
            cache[type] = new List<object>();
        cache[type].Add(comp);
    }

    public static bool Remove<T>(T comp) where T : Component
    {
        Type type = comp.GetType();
        if (cache.ContainsKey(type))
            return cache[type].Remove(comp);
        return false;
    }

    public static List<object> Get(Type type)
    {
        if (!cache.ContainsKey(type))
            cache[type] = new List<object>();
        return cache[type];
    }
}
