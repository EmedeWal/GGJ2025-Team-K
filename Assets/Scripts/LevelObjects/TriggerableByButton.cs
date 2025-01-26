using UnityEngine;

public class TriggerableByButton : MonoBehaviour
{
    [SerializeField] private triggerableType type;
    private Collider2D _collider;
    private SpriteRenderer _renderer;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        if (type == triggerableType.turnsUnsolid)
        {
            _renderer.color = Color.red;
        }
    }
    public enum triggerableType
    {
        exitDoor, //Exit door opens when triggered by button
        turnsUnsolid //Becomes non-solid when triggered
    }
    public void _onButtonPress()
    {
        switch (type)
        {
            case triggerableType.exitDoor:
                if (TryGetComponent<ExitDoor>(out ExitDoor door))
                {
                    door.unlocked = true;
                }
                break;
            case triggerableType.turnsUnsolid:
                //TODO: Change sprite here
                if (TryGetComponent<UnsolidPlatform>(out UnsolidPlatform platform))
                {
                    platform.changeColor(false);
                }
                _collider.enabled = false;
                break;
            default:
                break;
        }
    }

    public void _onButtonRelease()
    {
        switch (type)
        {
            case triggerableType.exitDoor:
                if (TryGetComponent<ExitDoor>(out ExitDoor door))
                {
                    door.unlocked = false;
                }
                break;
            case triggerableType.turnsUnsolid:
                //TODO: Change sprite here
                if (TryGetComponent<UnsolidPlatform>(out UnsolidPlatform platform))
                {
                    platform.changeColor(true);
                }
                _collider.enabled = true;
                break;
            default:
                break;
        }
    }
}
