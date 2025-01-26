using UnityEngine;

public class UnsolidPlatform : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    public void changeColor(bool red)
    {
        if (red)
        {
            sprite.color = Color.red;
        }
        else
        {
            sprite.color = Color.black;
        }
    }
}
