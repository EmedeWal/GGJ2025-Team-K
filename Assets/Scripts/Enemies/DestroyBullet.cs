using UnityEngine;

public class DestroyBullet : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
