using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("LEVEL BOUNDS")]
    [SerializeField] private BoxCollider2D _levelBounds;

    [Header("FOLLOW SETTINGS")]
    [SerializeField] private float _response = 5f;

    private Transform _followTarget;
    private Transform _transform;
    private Camera _camera;
    private float _cameraHalfWidth;

    private void Start()
    {
        _followTarget = FindFirstObjectByType<Controller>().transform;
        _transform = transform;

        _camera = GetComponentInChildren<Camera>();
        _cameraHalfWidth = _camera.orthographicSize * _camera.aspect;
    }

    private void LateUpdate()
    {
        var response = 1f - Mathf.Exp(-_response * Time.deltaTime);
        var targetX = Mathf.Lerp(_transform.position.x, _followTarget.position.x, response);
        var clampedX = Mathf.Clamp(targetX,
                                     _levelBounds.bounds.min.x + _cameraHalfWidth,
                                     _levelBounds.bounds.max.x - _cameraHalfWidth);

        _transform.position = new Vector3(clampedX, _transform.position.y, _transform.position.z);
    }
}