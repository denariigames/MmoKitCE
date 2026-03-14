using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

[RequireComponent(typeof(RectTransform))]
[DefaultExecutionOrder(1)]
public class UIFollowWorldPosition : MonoBehaviour
{
    public Camera targetCamera;
    public Vector3 targetPosition;
    public float damping = 5f;
    public float snapDistance = 100f;

    private Vector3? _wantedPosition;
    private TransformAccessArray _followJobTransforms;
    private UIFollowWorldPositionJob _followJob;
    private JobHandle _followJobHandle;

    public RectTransform CacheTransform { get; private set; }
    public Transform CacheCameraTransform { get; private set; }
    public Camera TargetCamera
    {
        get
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
                if (targetCamera != null)
                    CacheCameraTransform = targetCamera.transform;
            }
            return targetCamera;
        }
    }

    private void OnEnable()
    {
        CacheTransform = GetComponent<RectTransform>();
        _followJobTransforms = new TransformAccessArray(new Transform[] { CacheTransform });
    }

    private void OnDisable()
    {
        _followJobTransforms.Dispose();
        _followJobHandle.Complete();
    }

    private void Update()
    {
        if (TargetCamera == null)
            return;
        _wantedPosition = RectTransformUtility.WorldToScreenPoint(targetCamera, targetPosition);
    }

    private void LateUpdate()
    {
        if (!_wantedPosition.HasValue)
            return;
        _followJobHandle.Complete();
        _followJob = new UIFollowWorldPositionJob()
        {
            wantedPosition = _wantedPosition.Value,
            damping = damping,
            snapDistance = snapDistance,
            deltaTime = Time.deltaTime,
        };
        _followJobHandle = _followJob.Schedule(_followJobTransforms);
        JobHandle.ScheduleBatchedJobs();
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        if (TargetCamera == null)
            return;
        _wantedPosition = RectTransformUtility.WorldToScreenPoint(targetCamera, targetPosition);
        CacheTransform.position = _wantedPosition.Value;
    }
}

public struct UIFollowWorldPositionJob : IJobParallelForTransform
{
    public Vector2 wantedPosition;
    public float damping;
    public float snapDistance;
    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        if (damping <= 0f || Vector3.Distance(transform.position, wantedPosition) >= snapDistance)
            transform.position = wantedPosition;
        else
            transform.position = Vector3.Lerp(transform.position, wantedPosition, damping * deltaTime);
    }
}