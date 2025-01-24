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
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OnRelease();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        print("collision");
        if (collision.gameObject.layer == 8)
        {
            print("captured");
            _bubble = collision.gameObject;
            OnCapture();
        }
    }

    void OnCapture()
    {
        _bubble.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        var colour = GetComponentInChildren<SpriteRenderer>().color;
        colour.a = _transparency;
        GetComponentInChildren<SpriteRenderer>().color = colour;
        GetComponent<Collider2D>().enabled = false;
        transform.SetParent(_bubble.transform);
        transform.SetPositionAndRotation(_bubble.transform.position, Quaternion.identity);
        GetComponent<ApplyGravity>().IsInBubble = true;
    }

    void OnRelease()
    {
        transform.SetParent(null);
        GetComponent<ApplyGravity>().IsInBubble = false;
    }
}
