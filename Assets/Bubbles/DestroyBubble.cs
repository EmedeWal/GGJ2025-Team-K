using UnityEngine;

public class DestroyBubble : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (GetComponent<CaptureObject>().IsCaptured) GetComponent<CaptureObject>().OnRelease();
            Destroy(gameObject);
        }
    }
}
