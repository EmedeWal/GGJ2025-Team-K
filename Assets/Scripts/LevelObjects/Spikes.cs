using UnityEngine;

public class Spikes : MonoBehaviour
{

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
