using Unity.VisualScripting;
using UnityEngine;

public class CaptureObject : MonoBehaviour
{
    private GameObject _capturedObject;
    public GameObject CapturedObject
    {
        get => _capturedObject;
        set => _capturedObject = value;
    }

    [Range(0, 1)]
    [SerializeField] private float _transparency;

    private bool _isCaptured = false;
    public bool IsCaptured
    {
        get => _isCaptured;
        set => _isCaptured = value;
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        if (_isCaptured)
        {
            FollowBubble();
        }
        if (Input.GetKeyDown(KeyCode.E))
            OnRelease();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Capturable")
        {
            _capturedObject = collision.gameObject;
            OnCapture();
        }
    }

    void OnCapture()
    {
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        ChangeTransparency(_transparency);

        FollowBubble();

        _capturedObject.GetComponent<Collider2D>().enabled = false;
        _capturedObject.transform.SetParent(transform);
        _capturedObject.GetComponent<ApplyGravity>().IsInBubble = true;

        _isCaptured = true;
    }

    public void OnRelease()
    {
        ChangeTransparency(1);
        _capturedObject.GetComponent<Collider2D>().enabled = true;
        _capturedObject.transform.SetParent(null);
        _capturedObject.GetComponent<ApplyGravity>().IsInBubble = false;

        _isCaptured = false;
    }

    void ChangeTransparency(float newTransparency)
    {
        var renderer = _capturedObject.GetComponentInChildren<SpriteRenderer>();
        var colour = renderer.color;
        colour.a = newTransparency;
        renderer.color = colour;
    }

    void FollowBubble()
    {
        _capturedObject.transform.position = transform.position;
    }
}
