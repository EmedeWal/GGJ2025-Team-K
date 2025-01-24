using UnityEngine;

public class Spikes : MonoBehaviour
{
    /// <summary>
    /// Object that kills the player and enemies on contact, and pops bubbles
    /// </summary>
    /// <param name="collision"></param>

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IKillable killable))
        {
            killable.OnSpikeHit();
        }
    }
}

public interface IKillable
{
    public void OnSpikeHit();
}
