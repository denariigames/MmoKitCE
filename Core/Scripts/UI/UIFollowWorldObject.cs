using UnityEngine;

[RequireComponent(typeof(UIFollowWorldPosition))]
[DefaultExecutionOrder(0)]
public class UIFollowWorldObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;
    public Transform TargetObject
    {
        get { return targetObject; }
        set
        {
            targetObject = value;
            CachePositionFollower.SetTargetPosition(targetObject.position);
        }
    }

    public UIFollowWorldPosition CachePositionFollower { get; private set; }

    private void OnEnable()
    {
        CachePositionFollower = GetComponent<UIFollowWorldPosition>();
    }

    private void Update()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        if (TargetObject == null)
            return;

        CachePositionFollower.targetPosition = TargetObject.position;
    }
}
