using UnityEngine;
using System.Collections.Generic;

public class ExitDoor : MonoBehaviour
{
    private LevelManager manager;
    public bool unlocked = false;
    public doorType type = doorType.unlocked;

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
        LevelManager.ResetGameState += LevelManager_resetGameState;
    }

    private void LevelManager_resetGameState()
    {
        if (type != doorType.unlocked)
        {
            unlocked = false;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    void Update()
    {
        if (type == doorType.enemyLocked)
        {
            foreach (var enemy in enemies)
            {
                if (!enemy.GetComponent<BaseEnemy>().Enabled)
                {
                    enemiesLeft -= 1;
                }
            }
            if (enemiesLeft <= 0)
            {
                unlocked = true;
                GetComponent<SpriteRenderer>().color = Color.black;
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
