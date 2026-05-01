using System;
using System.Collections.Generic;
using System.Reflection;
#if !NET && !NETCOREAPP
using UnityEngine;
#endif

namespace Insthync.DevExtension
{
    public static class DevExtUtils
    {
        // NOTE: Use `Type` not `RuntimeTypeHandle` as the key because `RuntimeTypeHandle` comparison is slower than `Type` reference comparison, and `Type` is already cached by the runtime, so it won't cause additional memory usage.
        private static readonly Dictionary<Type, Dictionary<string, MethodInfo[]>> s_cacheDevExtMethods = new Dictionary<Type, Dictionary<string, MethodInfo[]>>();
        private const BindingFlags InstanceMethodBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags StaticMethodBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        /// <summary>
        /// This will calls all methods from `obj` that have attributes "DevExtMethodsAttribute("`baseMethodName`")" with any number of arguments that can be set via `args`
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="baseMethodName"></param>
        /// <param name="args"></param>
        public static void InvokeInstanceDevExtMethods<T>(this T obj, string baseMethodName, params object[] args)
        {
            if (obj == null)
                return;
            InvokeDevExtMethods(obj.GetType(), obj, baseMethodName, InstanceMethodBindingFlags, args);
        }

        public static T InvokeInstanceDevExtMethodsLoopItself<T>(this T obj, string baseMethodName, params object[] args)
        {
            if (obj == null)
                return obj;
            return (T)InvokeDevExtMethodsLoopItself(obj.GetType(), obj, baseMethodName, InstanceMethodBindingFlags, args);
        }

        /// <summary>
        /// This will calls all static methods which its type is `type` that have attributes "DevExtMethodsAttribute("`baseMethodName`")" with any number of arguments that can be set via `args`
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseMethodName"></param>
        /// <param name="args"></param>
        public static void InvokeStaticDevExtMethods(Type type, string baseMethodName, params object[] args)
        {
            InvokeDevExtMethods(type, null, baseMethodName, StaticMethodBindingFlags, args);
        }

        public static void CacheInstanceDevExtMethods(Type type, string baseMethodName)
        {
            TryGetDevExtMethods(type, baseMethodName, InstanceMethodBindingFlags, out _);
        }

        public static void CacheStaticDevExtMethods(Type type, string baseMethodName)
        {
            TryGetDevExtMethods(type, baseMethodName, StaticMethodBindingFlags, out _);
        }

        public static bool TryGetDevExtMethods(Type type, string baseMethodName, BindingFlags bindingFlags, out MethodInfo[] methods)
        {
            methods = null;
            Type typeHandle = type;

            if (!s_cacheDevExtMethods.TryGetValue(typeHandle, out var methodDict))
            {
                methodDict = new Dictionary<string, MethodInfo[]>();
                s_cacheDevExtMethods[typeHandle] = methodDict;
            }

            if (!methodDict.TryGetValue(baseMethodName, out methods))
            {
                var allMethods = type.GetMethods(bindingFlags); // allocates once
                int count = 0;

                // Use a preallocated temp array (worst-case: all methods match)
                MethodInfo[] tempMethods = new MethodInfo[allMethods.Length];

                for (int i = 0; i < allMethods.Length; i++)
                {
                    var attr = (DevExtMethodsAttribute)allMethods[i].GetCustomAttribute(typeof(DevExtMethodsAttribute), true);
                    if (attr != null && string.Equals(attr.BaseMethodName, baseMethodName))
                    {
                        tempMethods[count++] = allMethods[i];
                    }
                }

                if (count == 0)
                    methods = Array.Empty<MethodInfo>();
                else if (count == allMethods.Length)
                    methods = tempMethods; // all methods matched
                else
                {
                    methods = new MethodInfo[count];
                    Array.Copy(tempMethods, 0, methods, 0, count);
                }

                methodDict[baseMethodName] = methods;
            }

            return methods.Length > 0;
        }


        private static void InvokeDevExtMethods(Type type, object obj, string baseMethodName, BindingFlags bindingFlags, params object[] args)
        {
            try
            {
                if (!TryGetDevExtMethods(type, baseMethodName, bindingFlags, out MethodInfo[] methods))
                    return;

                for (int tempLoopCounter = 0; tempLoopCounter < methods.Length; ++tempLoopCounter)
                {
                    methods[tempLoopCounter].Invoke(obj, args);
                }
            }
            catch (Exception ex)
            {
#if NET || NETCOREAPP
                Console.WriteLine(ex.ToString());
#else
                Debug.LogException(ex);
#endif
            }
        }

        private static object InvokeDevExtMethodsLoopItself(Type type, object obj, string baseMethodName, BindingFlags bindingFlags, params object[] args)
        {
            try
            {
                if (!TryGetDevExtMethods(type, baseMethodName, bindingFlags, out MethodInfo[] methods))
                    return obj;

                for (int tempLoopCounter = 0; tempLoopCounter < methods.Length; ++tempLoopCounter)
                {
                    obj = methods[tempLoopCounter].Invoke(obj, args);
                }
            }
            catch (Exception ex)
            {
#if NET || NETCOREAPP
                Console.WriteLine(ex.ToString());
#else
                Debug.LogException(ex);
#endif
            }
            return obj;
        }
    }
}
