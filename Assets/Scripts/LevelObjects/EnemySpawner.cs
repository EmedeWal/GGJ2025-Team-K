using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject redKoopaPrefab;

    [SerializeField] private string enemyType = "redKoopa"; //Type of enemy this spawns
    [SerializeField] private int enemyId = 0; //Used to match this with the enemy it spawns
    [SerializeField] private RedKoopa matchedEnemy;

    void Start()
    {
        switch (enemyType)
        {
            case "redKoopa":
                break;
            default:
                break;
        }
        
    }

    void Update()
    {
        if (matchedEnemy == null)
        {
            GameObject newEnemy;
            switch (enemyType)
            {
                case "redKoopa":
                    newEnemy = Instantiate(redKoopaPrefab);
                    newEnemy.transform.position = transform.position;
                    newEnemy.GetComponent<RedKoopa>().hasSpawner = true;
                    newEnemy.GetComponent<RedKoopa>()._spawner = this;
                    matchedEnemy = newEnemy.GetComponent<RedKoopa>();
                    break;
                default:
                    break;
            }
        }
    }
}
