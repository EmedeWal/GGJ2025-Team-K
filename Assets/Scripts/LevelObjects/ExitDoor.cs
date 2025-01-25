using UnityEngine;
using System.Collections.Generic;

public class ExitDoor : MonoBehaviour
{
    private LevelManager manager;
    public bool unlocked = false;
    [SerializeField] private doorType type = doorType.unlocked;

    private int startingEnemies;
    public int enemiesLeft = 3;
    [SerializeField] private List<GameObject> enemies;

    public enum doorType
    {
        unlocked,
        powerLocked,
        enemyLocked
    }

    void Start()
    {
        startingEnemies = enemiesLeft;
        manager = FindAnyObjectByType<LevelManager>();
    }

    void Update()
    {
        if (type == doorType.enemyLocked)
        {
            foreach (GameObject x in enemies)
            {
                if (x == null)
                {
                    enemiesLeft -= 1;
                }
            }
            if (enemiesLeft <= 0)
            {
                unlocked = true;
            }
            enemiesLeft = startingEnemies;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == doorType.unlocked || unlocked)
        if (collision.gameObject.TryGetComponent(out Controller controller))
        {
            //Maybe do a lil animator animation here of the character shrinking/getting darker to show them going to the next level before switching levels
            manager.ChangeLevels(manager.currentLevel + 1);
        }
    }
}
