using UnityEngine;

public class KeepInScreenBounds : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Vector2 _defaultAnchoredPosition;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        if (_rectTransform == null || _canvas == null)
        {
            Debug.LogError("RectTransform or Canvas is missing!");
            enabled = false;
            return;
        }

        _defaultAnchoredPosition = _rectTransform.anchoredPosition;
    }

    void Update()
    {
        KeepRectTransformInBounds();
    }

    private void KeepRectTransformInBounds()
    {
        // Get the corners of the RectTransform in world space
        Vector3[] corners = new Vector3[4];
        _rectTransform.GetWorldCorners(corners);

        // Get the canvas rect
        RectTransform canvasRectTransform = _canvas.GetComponent<RectTransform>();
        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetWorldCorners(canvasCorners);

        // Calculate new position
        Vector3 newPosition = _rectTransform.position;
        bool isOutOfBounds = false;

        if (corners[0].x < canvasCorners[0].x)
        {
            newPosition.x += canvasCorners[0].x - corners[0].x;
            isOutOfBounds = true;
        }
        if (corners[2].x > canvasCorners[2].x)
        {
            newPosition.x -= corners[2].x - canvasCorners[2].x;
            isOutOfBounds = true;
        }

        if (corners[0].y < canvasCorners[0].y)
        {
            newPosition.y += canvasCorners[0].y - corners[0].y;
            isOutOfBounds = true;
        }
        if (corners[1].y > canvasCorners[1].y)
        {
            newPosition.y -= corners[1].y - canvasCorners[1].y;
            isOutOfBounds = true;
        }

        // If out of bounds, adjust the position
        if (isOutOfBounds)
        {
            _rectTransform.position = newPosition;
        }
        else
        {
            // If not out of bounds, reset to default anchored position
            _rectTransform.anchoredPosition = _defaultAnchoredPosition;
        }
    }
}
