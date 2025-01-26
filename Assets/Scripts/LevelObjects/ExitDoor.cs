using UnityEngine;
using System.Collections.Generic;

public class ExitDoor : MonoBehaviour
{
    private LevelManager manager;
    public bool unlocked = false;
    [SerializeField] private doorType type = doorType.unlocked;

    private int startingEnemies;
    public int enemiesLeft = 3;
    [SerializeField] private List<BaseEnemy> enemies;

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
            foreach (BaseEnemy enemy in enemies)
            {
                if (!enemy.Enabled)
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
