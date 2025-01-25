using UnityEngine;

public class TriggerableByButton : MonoBehaviour
{
    [SerializeField] private triggerableType type;
    private Collider2D collider;

    private void Start()
    {
        collider = GetComponent<Collider2D>();
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
                collider.enabled = false;
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
                break;
            case triggerableType.turnsUnsolid:
                //TODO: Change sprite here
                collider.enabled = true;
                break;
            default:
                break;
        }
    }
}
