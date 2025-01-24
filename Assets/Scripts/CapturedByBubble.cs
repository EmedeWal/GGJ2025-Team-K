using Unity.VisualScripting;
using UnityEngine;

public class CapturedByBubble : MonoBehaviour
{
    private GameObject _bubble;
    public GameObject Bubble
    {
        get => _bubble;
        set => _bubble = value;
    }

    [Range(0, 1)]
    [SerializeField] private float _transparency = 0.7f;

    private bool _isCaptured = false;
    
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
        if (collision.gameObject.layer == 8)
        {
            _bubble = collision.gameObject;
            OnCapture();
        }
    }

    void OnCapture()
    {
        _bubble.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        ChangeTransparency(_transparency);

        FollowBubble();

        GetComponent<Collider2D>().enabled = false;
        transform.SetParent(_bubble.transform);
        GetComponent<ApplyGravity>().IsInBubble = true;
    }

    void OnRelease()
    {
        ChangeTransparency(1);
        GetComponent<Collider2D>().enabled = true;
        transform.SetParent(null);
        GetComponent<ApplyGravity>().IsInBubble = false;
    }

    void ChangeTransparency(float newTransparency)
    {
        var renderer = GetComponentInChildren<SpriteRenderer>();
        var colour = renderer.color;
        colour.a = newTransparency;
        renderer.color = colour;
    }

    void FollowBubble()
    {
        transform.position = _bubble.transform.position;
    }
}
