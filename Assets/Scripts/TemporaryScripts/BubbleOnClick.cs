using UnityEngine;
using UnityEngine.InputSystem;

public class BubbleOnClick : MonoBehaviour
{
    [SerializeField] private GameObject _bubblePrefab;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnBubble();
        }
    }

    void SpawnBubble()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        print(mousePosition);
        var bubble = Instantiate(_bubblePrefab, mousePosition, Quaternion.identity);
    }
}
