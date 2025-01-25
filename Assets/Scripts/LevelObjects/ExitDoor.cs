using UnityEngine;
using System.Collections.Generic;

public class ExitDoor : MonoBehaviour
{
    private LevelManager manager;
    public bool unlocked = false;
    [SerializeField] private doorType type = doorType.unlocked;

    //[SerializeField] private List<>

    public enum doorType
    {
        unlocked,
        powerLocked,
        enemyLocked
    }

    void Start()
    {
        manager = FindAnyObjectByType<LevelManager>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == doorType.unlocked || unlocked)
        if (collision.gameObject.TryGetComponent(out Controller controller))
        {
            //Maybe do a lil animator animation here of the character shrinking/getting darker to show them going to the next level before switching levels
            manager.changeLevels(manager.currentLevel + 1);
        }
    }
}
