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

    void OnCollisionEnter(Collision collision)
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
        transform.SetParent(_bubble.transform);
        GetComponent<ApplyGravity>().IsInBubble = true;
    }

    void OnRelease()
    {
        transform.SetParent(null);
        GetComponent<ApplyGravity>().IsInBubble = false;
    }
}
