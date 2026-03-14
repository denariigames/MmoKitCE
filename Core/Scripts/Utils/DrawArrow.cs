using UnityEngine;

/// <summary>
/// Based on  https://forum.unity3d.com/threads/debug-drawarrow.85980/
/// </summary>
public static class DrawArrow
{
    public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowLength = 1f, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowNormalizedPosition = 1f)
    {
        ForGizmo(pos, direction, Gizmos.color, arrowLength, arrowHeadLength, arrowHeadAngle, arrowNormalizedPosition);
    }

    public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowLength = 1f, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowNormalizedPosition = 1f)
    {
        ForGizmoTwoPoints(pos, pos + (direction * arrowLength), color, arrowHeadLength, arrowHeadAngle, arrowNormalizedPosition);
    }

    public static void ForGizmoTwoPoints(Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowNormalizedPosition = 1f)
    {
        ForGizmoTwoPoints(from, to, Gizmos.color, arrowHeadLength, arrowHeadAngle, arrowNormalizedPosition);
    }

    public static void ForGizmoTwoPoints(Vector3 from, Vector3 to, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowNormalizedPosition = 1f)
    {
        Color prevColor = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawLine(from, to);
        Vector3 direction = to - from;
        DrawArrowEnd(true, from, direction, color, arrowHeadLength, arrowHeadAngle, arrowNormalizedPosition);
        Gizmos.color = prevColor;
    }

    public static void ForDebug(Vector3 pos, Vector3 direction, float arrowLength = 1f, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowNormalizedPosition = 1f)
    {
        ForDebug(pos, direction, Color.white, arrowLength, arrowHeadLength, arrowHeadAngle, arrowNormalizedPosition);
    }

    public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowLength = 1f, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowNormalizedPosition = 1f)
    {
        ForDebugTwoPoints(pos, pos + (direction * arrowLength), color, arrowHeadLength, arrowHeadAngle, arrowNormalizedPosition);
    }

    public static void ForDebugTwoPoints(Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowNormalizedPosition = 1f)
    {
        ForDebugTwoPoints(from, to, Color.white, arrowHeadLength, arrowHeadAngle, arrowNormalizedPosition);
    }

    public static void ForDebugTwoPoints(Vector3 from, Vector3 to, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowNormalizedPosition = 1f)
    {
        Debug.DrawLine(from, to, color);
        Vector3 direction = to - from;
        DrawArrowEnd(false, from, direction, color, arrowHeadLength, arrowHeadAngle, arrowNormalizedPosition);
    }

    private static void DrawArrowEnd(bool gizmos, Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowNormalizedPosition = 1f)
    {
        Vector3 right = (Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back) * arrowHeadLength;
        Vector3 left = (Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back) * arrowHeadLength;
        Vector3 up = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back) * arrowHeadLength;
        Vector3 down = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back) * arrowHeadLength;

        Vector3 arrowTip = pos + (direction * arrowNormalizedPosition);

        if (gizmos)
        {
            Color prevColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawRay(arrowTip, right);
            Gizmos.DrawRay(arrowTip, left);
            Gizmos.DrawRay(arrowTip, up);
            Gizmos.DrawRay(arrowTip, down);
            Gizmos.color = prevColor;
        }
        else
        {
            Debug.DrawRay(arrowTip, right, color);
            Debug.DrawRay(arrowTip, left, color);
            Debug.DrawRay(arrowTip, up, color);
            Debug.DrawRay(arrowTip, down, color);
        }
    }
}