using Cysharp.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static partial class GenericUtils
{
    public static bool IsFocusInputField()
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            InputField inputField = EventSystem.current.currentSelectedGameObject.GetComponent<InputField>();
            if (inputField != null && inputField.isFocused)
                return true;
            TMP_InputField tmpInputField = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
            if (tmpInputField != null && tmpInputField.isFocused)
                return true;
        }
        return false;
    }

    public static Rect GetWorldRect(this RectTransform transform)
    {
        Vector3[] corners = new Vector3[4];
        transform.GetWorldCorners(corners);
        // Get the bottom left corner.
        Vector3 position = corners[0];
        Vector2 size = new Vector2(transform.lossyScale.x * transform.rect.size.x, transform.lossyScale.y * transform.rect.size.y);
        return new Rect(position, size);
    }

    public static bool ContainsRect(this Rect rect1, Rect rect2, float rect2Scale = 1f)
    {
        return rect1.Contains(rect2.center + (new Vector2(-rect2.size.x, -rect2.size.y) * 0.5f * rect2Scale)) &&
            rect1.Contains(rect2.center + (new Vector2(rect2.size.x, -rect2.size.y) * 0.5f * rect2Scale)) &&
            rect1.Contains(rect2.center + (new Vector2(-rect2.size.x, rect2.size.y) * 0.5f * rect2Scale)) &&
            rect1.Contains(rect2.center + (new Vector2(rect2.size.x, rect2.size.y) * 0.5f * rect2Scale));
    }

    public static int GetRandomIndexInArray<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
            return 0;
        return Random.Range(0, list.Count);
    }

    public static T GetRandomObjectInArray<T>(this IList<T> list, out int index)
    {
        index = -1;
        if (list == null || list.Count == 0)
            return default;
        index = Random.Range(0, list.Count);
        return list[index];
    }

    public static void SetLayerRecursively(this GameObject gameObject, int layerIndex, bool includeInactive)
    {
        if (gameObject == null) return;
        SetLayerInternal(gameObject.transform, layerIndex, includeInactive);
    }

    private static void SetLayerInternal(Transform root, int layerIndex, bool includeInactive)
    {
        GameObject go = root.gameObject;
        if (go.layer != layerIndex)
            go.layer = layerIndex;

        int childCount = root.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = root.GetChild(i);

            if (includeInactive || child.gameObject.activeSelf)
                SetLayerInternal(child, layerIndex, includeInactive);
        }
    }

    public static List<T> GetComponents<T>(this IEnumerable<GameObject> gameObjects) where T : Component
    {
        List<T> result = new List<T>();
        T comp;
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject == null)
                continue;
            if (gameObject.TryGetComponent(out comp))
                result.Add(comp);
        }
        return result;
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject == null)
            return null;
        T result;
        if (!gameObject.TryGetComponent(out result))
            result = gameObject.AddComponent<T>();
        return result;
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject, System.Action<T> onAddComponent) where T : Component
    {
        if (gameObject == null)
            return null;
        T result;
        if (!gameObject.TryGetComponent(out result))
        {
            result = gameObject.AddComponent<T>();
            onAddComponent.Invoke(result);
        }
        return result;
    }

    public static TInterface GetOrAddComponent<TInterface, T>(this GameObject gameObject)
        where T : Component, TInterface
    {
        if (gameObject == null)
            return default;
        TInterface result;
        if (!gameObject.TryGetComponent(out result))
            result = gameObject.AddComponent<T>();
        return result;
    }

    public static TInterface GetOrAddComponent<TInterface, T>(this GameObject gameObject, System.Action<TInterface> onAddComponent)
        where T : Component, TInterface
    {
        if (gameObject == null)
            return default;
        TInterface result;
        if (!gameObject.TryGetComponent(out result))
        {
            result = gameObject.AddComponent<T>();
            onAddComponent.Invoke(result);
        }
        return result;
    }

    public static void RemoveChildren(this Transform transform, bool immediatelyMode = false)
    {
        transform.DestroyChildren(immediatelyMode);
    }

    public static void DestroyChildren(this Transform transform, bool immediatelyMode = false)
    {
        if (transform == null)
            return;
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            Transform lastChild = transform.GetChild(i);
            if (!immediatelyMode)
                Object.Destroy(lastChild.gameObject);
            else
                Object.DestroyImmediate(lastChild.gameObject);
        }
    }

    public static void SetChildrenActive(this Transform transform, bool isActive)
    {
        if (transform == null)
            return;
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(isActive);
        }
    }

    public static void RemoveObjectsByComponentInChildren<T>(this GameObject gameObject, bool includeInactive) where T : Component
    {
        if (gameObject == null)
            return;
        T[] components = gameObject.GetComponentsInChildren<T>(includeInactive);
        foreach (T component in components)
        {
            Object.DestroyImmediate(component.gameObject);
        }
    }

    public static void RemoveObjectsByComponentInParent<T>(this GameObject gameObject, bool includeInactive) where T : Component
    {
        if (gameObject == null)
            return;
        T[] components = gameObject.GetComponentsInParent<T>(includeInactive);
        foreach (T component in components)
        {
            Object.DestroyImmediate(component.gameObject);
        }
    }

    public static void RemoveComponents<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject == null)
            return;
        T[] components = gameObject.GetComponents<T>();
        foreach (T component in components)
        {
            Object.DestroyImmediate(component);
        }
    }

    public static void RemoveComponentsInChildren<T>(this GameObject gameObject, bool includeInactive) where T : Component
    {
        if (gameObject == null)
            return;
        T[] components = gameObject.GetComponentsInChildren<T>(includeInactive);
        foreach (T component in components)
        {
            Object.DestroyImmediate(component);
        }
    }

    public static void RemoveComponentsInParent<T>(this GameObject gameObject, bool includeInactive) where T : Component
    {
        if (gameObject == null)
            return;
        T[] components = gameObject.GetComponentsInParent<T>(includeInactive);
        foreach (T component in components)
        {
            Object.DestroyImmediate(component);
        }
    }

    public static List<T> GetComponentsFromAllLoadedScenes<T>(bool includeInactive)
        where T : Component
    {
        List<T> result = new List<T>();
        for (int i = 0; i < SceneManager.loadedSceneCount; ++i)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            for (int j = 0; j < rootGameObjects.Length; ++j)
            {
                if (!rootGameObjects[j].activeSelf && !includeInactive)
                    continue;
                result.AddRange(rootGameObjects[j].GetComponentsInChildren<T>(includeInactive));
            }
        }
        return result;
    }

    public static List<GameObject> GetGameObjectsFromAllLoadedScenes()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            foreach (GameObject rootObject in rootGameObjects)
            {
                gameObjects.AddRange(GetAllChildGameObjects(rootObject));
            }
        }
        return gameObjects;
    }

    public static List<GameObject> GetAllChildGameObjects(GameObject parent)
    {
        List<GameObject> allObjects = new List<GameObject> { parent };
        foreach (Transform child in parent.transform)
        {
            allObjects.AddRange(GetAllChildGameObjects(child.gameObject));
        }
        return allObjects;
    }

    public static int GetNegativePositive()
    {
        return Random.value > 0.5f ? 1 : -1;
    }

    public static void SetAndStretchToParentSize(this RectTransform rect, RectTransform parentRect)
    {
        rect.SetParent(parentRect);
        rect.localScale = Vector2.one;
        rect.anchoredPosition = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = Vector2.one * 0.5f;
        rect.sizeDelta = Vector3.zero;
    }

    public static Color SetAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    public static Vector3 GetXZ(this Vector3 position)
    {
        return new Vector3(position.x, 0f, position.z);
    }

    public static Vector3 GetXY(this Vector3 position)
    {
        return new Vector3(position.x, position.y, 0f);
    }

    public static Vector2 GetVector2(this Vector3 position)
    {
        return new Vector2(position.x, position.y);
    }

    public static bool IsPointInBox(Vector3 center, Vector3 half, Vector3 dirX, Vector3 dirY, Vector3 dirZ, Vector3 point)
    {
        Vector3 d = point - center;
        return Mathf.Abs(Vector3.Dot(d, dirX)) <= half.x &&
            Mathf.Abs(Vector3.Dot(d, dirY)) <= half.y &&
            Mathf.Abs(Vector3.Dot(d, dirZ)) <= half.z;
    }

    public static bool ColliderIntersect(this Collider source, Collider dest, float sourceSizeRate = 1f)
    {
        source.GetActualSizeBoundsPoints(out Vector3 sourceSize, out Vector3[] sourcePoints);
        Vector3 center = (sourcePoints[0] + sourcePoints[7]) * 0.5f;
        Vector3 size = sourceSize * sourceSizeRate;
        Collider[] results = Physics.OverlapBox(center, size * 0.5f, Quaternion.identity, LayerMask.GetMask(LayerMask.LayerToName(dest.gameObject.layer)), QueryTriggerInteraction.Collide);
        for (int i = 0; i < results.Length; ++i)
        {
            if (results[i] == dest)
                return true;
        }
        return false;
    }

    public static bool ColliderIntersect(this Collider2D source, Collider2D dest, float sourceSizeRate = 1f)
    {
        source.GetActualSizeBoundsPoints(out Vector3 sourceSize, out Vector3[] sourcePoints);
        Vector3 center = (sourcePoints[0] + sourcePoints[7]) * 0.5f;
        Vector3 size = sourceSize * sourceSizeRate;
        Collider2D[] results = Physics2D.OverlapBoxAll(center, size, source.transform.eulerAngles.z, LayerMask.GetMask(LayerMask.LayerToName(dest.gameObject.layer)));
        for (int i = 0; i < results.Length; ++i)
        {
            if (results[i] == dest)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Get OBB size and points
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="size"></param>
    /// <param name="points"></param>
    public static void GetActualSizeBoundsPoints(this Collider collider, out Vector3 size, out Vector3[] points)
    {
        points = new Vector3[8];
        Vector3[] sourcePoints = new Vector3[8];
        Transform transform = collider.transform;

        // Store original rotation
        Quaternion originalRotation = transform.rotation;

        // Reset rotation
        transform.rotation = Quaternion.identity;

        // Get object bounds from unrotated object
        Bounds bounds = collider.bounds;
        size = bounds.size;

        // Get the unrotated points
        sourcePoints[0] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z) - transform.position; // Bot left near
        sourcePoints[1] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z) - transform.position; // Bot right near
        sourcePoints[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z) - transform.position; // Top left near
        sourcePoints[3] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z) - transform.position; // Top right near
        sourcePoints[4] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z) - transform.position; // Bot left far
        sourcePoints[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z) - transform.position; // Bot right far
        sourcePoints[6] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z) - transform.position; // Top left far
        sourcePoints[7] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z) - transform.position; // Top right far

        // Apply scaling
        for (int i = 0; i < sourcePoints.Length; i++)
        {
            sourcePoints[i] = new Vector3(sourcePoints[i].x / transform.localScale.x,
                sourcePoints[i].y / transform.localScale.y,
                sourcePoints[i].z / transform.localScale.z);
        }

        // Restore rotation
        transform.rotation = originalRotation;

        // Transform points from local to world space
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = transform.TransformPoint(sourcePoints[i]);
        }
    }

    /// <summary>
    /// Get OBB size and points
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="size"></param>
    /// <param name="points"></param>
    public static void GetActualSizeBoundsPoints(this Collider2D collider, out Vector3 size, out Vector3[] points)
    {
        points = new Vector3[8];
        Vector3[] sourcePoints = new Vector3[8];
        Transform transform = collider.transform;

        // Store original rotation
        Quaternion originalRotation = transform.rotation;

        // Reset rotation
        transform.rotation = Quaternion.identity;

        // Get object bounds from unrotated object
        Bounds bounds = collider.bounds;
        size = bounds.size;

        // Get the unrotated points
        sourcePoints[0] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z) - transform.position; // Bot left near
        sourcePoints[1] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z) - transform.position; // Bot right near
        sourcePoints[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z) - transform.position; // Top left near
        sourcePoints[3] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z) - transform.position; // Top right near
        sourcePoints[4] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z) - transform.position; // Bot left far
        sourcePoints[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z) - transform.position; // Bot right far
        sourcePoints[6] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z) - transform.position; // Top left far
        sourcePoints[7] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z) - transform.position; // Top right far

        // Apply scaling
        for (int i = 0; i < sourcePoints.Length; i++)
        {
            sourcePoints[i] = new Vector3(sourcePoints[i].x / transform.localScale.x,
                sourcePoints[i].y / transform.localScale.y,
                sourcePoints[i].z / transform.localScale.z);
        }

        // Restore rotation
        transform.rotation = originalRotation;

        // Transform points from local to world space
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = transform.TransformPoint(sourcePoints[i]);
        }
    }

    public static bool IsError(this UnityWebRequest unityWebRequest)
    {
#if UNITY_2020_2_OR_NEWER
        UnityWebRequest.Result result = unityWebRequest.result;
        return (result == UnityWebRequest.Result.ConnectionError)
            || (result == UnityWebRequest.Result.DataProcessingError)
            || (result == UnityWebRequest.Result.ProtocolError);
#else
        return unityWebRequest.isHttpError || unityWebRequest.isNetworkError;
#endif
    }

    public static void DrawSquareGizmos(Transform transform, Color color, float squareSizeX, float squareSizeZ)
    {
        // Set color
        Color defaultColor = Gizmos.color;
        Gizmos.color = color;
        // 1--2
        // |  |
        // 3--4
        Vector3 p1 = new Vector3(transform.position.x - squareSizeX / 2, transform.position.y, transform.position.z - squareSizeZ / 2);
        Vector3 p2 = new Vector3(transform.position.x + squareSizeX / 2, transform.position.y, transform.position.z - squareSizeZ / 2);
        Vector3 p3 = new Vector3(transform.position.x - squareSizeX / 2, transform.position.y, transform.position.z + squareSizeZ / 2);
        Vector3 p4 = new Vector3(transform.position.x + squareSizeX / 2, transform.position.y, transform.position.z + squareSizeZ / 2);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p4);
        Gizmos.DrawLine(p4, p3);
        Gizmos.DrawLine(p3, p1);
        // Restore default colors
        Gizmos.color = defaultColor;
    }

    public static void DrawCircleGizmos(Transform transform, Color color, float radius)
    {
        // Set matrix
        Matrix4x4 defaultMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        // Set color
        Color defaultColor = Gizmos.color;
        Gizmos.color = color;
        // Draw a ring
        Vector3 beginPoint = Vector3.zero;
        Vector3 firstPoint = Vector3.zero;
        for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.1f)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 endPoint = new Vector3(x, 0, z);
            if (theta == 0)
            {
                firstPoint = endPoint;
            }
            else
            {
                Gizmos.DrawLine(beginPoint, endPoint);
            }
            beginPoint = endPoint;
        }
        // Draw the last segment
        Gizmos.DrawLine(firstPoint, beginPoint);
        // Restore default colors
        Gizmos.color = defaultColor;
        // Restore default matrix
        Gizmos.matrix = defaultMatrix;
    }

    public static string ToBonusString(this short value, string format = "N0")
    {
        return value >= 0 ? ZString.Concat("+", value.ToString(format)) : value.ToString(format);
    }

    public static string ToBonusString(this int value, string format = "N0")
    {
        return value >= 0 ? ZString.Concat("+", value.ToString(format)) : value.ToString(format);
    }

    public static string ToBonusString(this float value, string format = "N0")
    {
        return value >= 0 ? ZString.Concat("+", value.ToString(format)) : value.ToString(format);
    }

    public static string GetPrettyDate(this System.TimeSpan dateTimeDiff, bool future = false)
    {
        // TODO: Will get format string from language settings
        string textAFewSecondsAgo = "a few seconds ago";
        string textAMinuteAgo = "1 Minute ago";
        string textAHourAgo = "1 Hour ago";
        string textYesterday = "Yesterday";
        string formatMinutesAgo = "{0} Minutes ago";
        string formatHoursAgo = "{0} Hours ago";
        string formatDaysAgo = "{0} Days ago";
        string formatWeeksAgo = "{0} Weeks ago";
        string formatMonthsAgo = "{0} Months ago";
        string textUnknow = "Unknown";

        // Future text
        string textAFewSeconds = "a few seconds";
        string textAMinute = "1 Minute";
        string textAHour = "1 Hour";
        string textTomorrow = "Tomorrow";
        string formatMinutes = "{0} Minutes";
        string formatHours = "{0} Hours";
        string formatDays = "{0} Days";
        string formatWeeks = "{0} Weeks";
        string formatMonths = "{0} Months";

        int monthDiff = (int)(dateTimeDiff.TotalDays / 30);
        int dayDiff = (int)dateTimeDiff.TotalDays;
        int secDiff = (int)dateTimeDiff.TotalSeconds;

        if (future)
        {
            return GetPrettyDate(monthDiff, dayDiff, secDiff, textAFewSeconds, textAMinute, textAHour, textTomorrow, formatMinutes, formatHours, formatDays, formatWeeks, formatMonths, textUnknow);
        }
        else
        {
            return GetPrettyDate(monthDiff, dayDiff, secDiff, textAFewSecondsAgo, textAMinuteAgo, textAHourAgo, textYesterday, formatMinutesAgo, formatHoursAgo, formatDaysAgo, formatWeeksAgo, formatMonthsAgo, textUnknow);
        }
    }

    public static string GetPrettyDate(
        int monthDiff,
        int dayDiff,
        int secDiff,
        string textNow,
        string textAMinute,
        string textAHour,
        string textADay,
        string formatMinutes,
        string formatHours,
        string formatDays,
        string formatWeeks,
        string formatMonths,
        string textUnknow)
    {

        // Don't allow out of range values.
        if (dayDiff < 0)
            return textUnknow;

        // Handle same-day times.
        if (dayDiff == 0)
        {
            // Less than one minute.
            if (secDiff < 60)
                return textNow;
            // Less than 2 minutes.
            if (secDiff < 120)
                return textAMinute;
            // Less than one hour.
            if (secDiff < 3600)
                return ZString.Format(formatMinutes, Mathf.CeilToInt((float)secDiff / 60f));
            // Less than 2 hours.
            if (secDiff < 7200)
                return textAHour;
            // Less than one day.
            if (secDiff < 86400)
                return ZString.Format(formatHours, Mathf.CeilToInt((float)secDiff / 3600f));
        }
        // Handle previous days.
        if (dayDiff == 1)
            return textADay;
        if (dayDiff < 7)
            return ZString.Format(formatDays, dayDiff);
        if (dayDiff < 30)
            return ZString.Format(formatWeeks, Mathf.CeilToInt((float)dayDiff / 7f));
        if (monthDiff < 12)
            return ZString.Format(formatMonths, monthDiff);

        return textUnknow;
    }

    public static System.Uri Append(this System.Uri uri, params string[] paths)
    {
        return new System.Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => ZString.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/'))));
    }

    public static void Nullify<T>(this IList<T> objects)
        where T : class
    {
        if (objects == null)
            return;
        for (int i = 0; i < objects.Count; ++i)
        {
            objects[i] = null;
        }
    }

    public static void SetDefaultValues<T>(this IList<T> objects)
        where T : struct
    {
        if (objects == null)
            return;
        for (int i = 0; i < objects.Count; ++i)
        {
            objects[i] = default;
        }
    }

    public static void DestroyAndNullify<T>(this IList<T> objects)
        where T : Object
    {
        if (objects == null)
            return;
        for (int i = 0; i < objects.Count; ++i)
        {
            Object.Destroy(objects[i]);
            objects[i] = null;
        }
    }

    public static bool IsPositionInFov2D(this Vector2 origin, float fov, Vector2 position, Vector2 forward)
    {
        Vector2 targetDir = position - origin;
        targetDir.Normalize();
        // Angle in forward position is 180 so we use this value to determine that target is in hit fov or not
        return Vector2.Angle(targetDir, forward) < fov * 0.5f;
    }

    public static bool IsPositionInFov3D(this Vector3 origin, float fov, Vector3 position, Vector3 forward)
    {
        // This is unsigned angle, so angle found from this function is 0 - 180
        // if position forward from character this value will be 180
        // so just find for angle > 180 - halfFov
        Vector3 targetDir = position - origin;
        targetDir.y = 0;
        forward.y = 0;
        targetDir.Normalize();
        forward.Normalize();
        return Vector3.Angle(targetDir, forward) < fov * 0.5f;
    }

    static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
    public static string SizeSuffix(long value, int decimalPlaces = 1)
    {
        if (decimalPlaces < 0) { throw new System.ArgumentOutOfRangeException("decimalPlaces"); }
        if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
        if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} " + SizeSuffixes[0], 0); }

        // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
        int mag = (int)System.Math.Log(value, 1024);

        // 1L << (mag * 10) == 2 ^ (10 * mag) 
        // [i.e. the number of bytes in the unit corresponding to mag]
        decimal adjustedSize = (decimal)value / (1L << (mag * 10));

        // make adjustment when the value is large enough that
        // it would round up to 1000 or more
        if (System.Math.Round(adjustedSize, decimalPlaces) >= 1000)
        {
            mag += 1;
            adjustedSize /= 1024;
        }

        return string.Format("{0:n" + decimalPlaces + "} {1}",
            adjustedSize,
            SizeSuffixes[mag]);
    }

    public static string MinMaxSizeSuffix(long min, long max, int decimalPlaces = 1)
    {
        if (decimalPlaces < 0) { throw new System.ArgumentOutOfRangeException("decimalPlaces"); }
        if (max < 0) { return "-" + MinMaxSizeSuffix(min, -max, decimalPlaces); }
        if (max == 0) { return string.Format("{0:n" + decimalPlaces + "}{1:n" + decimalPlaces + "} " + SizeSuffixes[0], 0, 0); }

        // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
        int mag = (int)System.Math.Log(max, 1024);

        // 1L << (mag * 10) == 2 ^ (10 * mag) 
        // [i.e. the number of bytes in the unit corresponding to mag]
        decimal adjustedMinSize = (decimal)min / (1L << (mag * 10));
        decimal adjustedMaxSize = (decimal)max / (1L << (mag * 10));

        // make adjustment when the value is large enough that
        // it would round up to 1000 or more
        if (System.Math.Round(adjustedMaxSize, decimalPlaces) >= 1000)
        {
            mag += 1;
            adjustedMinSize /= 1024;
            adjustedMaxSize /= 1024;
        }

        return string.Format("{0:n" + decimalPlaces + "}/{1:n" + decimalPlaces + "} {2}",
            adjustedMinSize,
            adjustedMaxSize,
            SizeSuffixes[mag]);
    }

    public static async UniTask FrameBasedDelay(float duration, CancellationToken cancellationToken)
    {
        while (duration > 0f)
        {
            await UniTask.Yield(cancellationToken);
            duration -= Time.unscaledDeltaTime;
        }
    }
}
