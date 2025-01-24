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
        var colour = _bubble.GetComponentInChildren<SpriteRenderer>().color;
        colour.a = 0.5f;
        _bubble.GetComponentInChildren<SpriteRenderer>().color = colour;
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
