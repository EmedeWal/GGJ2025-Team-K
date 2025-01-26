using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    public static event Action ResetGameState; //This should be put on every movable object or alterable object so it resets when resetting or changing levels

    [SerializeField] private CameraController _camera;
    //[SerializeField] private CinemachineCamera cineCamera;
    private Controller player;

    public int currentLevel = 1;
    public bool staticCamera = true;

    [SerializeField] private List<GameObject> playerSpawns;
    
    [SerializeField] private Collider2D bounds1;
    [SerializeField] private Collider2D bounds2;
    [SerializeField] private Collider2D bounds3;
    [SerializeField] private Collider2D bounds4;
    [SerializeField] private Collider2D bounds5;
    [SerializeField] private Collider2D bounds6;

    void Start()
    {
        _camera = FindAnyObjectByType<CameraController>();
        //cineCamera = FindAnyObjectByType<CinemachineCamera>();
        player = FindAnyObjectByType<Controller>();
        ChangeLevels(1);
    }

    private void OnDisable()
    {
        ResetGameState = null;
    }

    private void Update()
    {
        //DEBUG TOOLS to go to the next or previous level
        if (Input.GetKeyDown(KeyCode.O))
            ChangeLevels(currentLevel + 1);
        else if (Input.GetKeyDown(KeyCode.Backspace))
            ChangeLevels(currentLevel - 1);
        else if (Input.GetKeyDown(KeyCode.R))
            FindFirstObjectByType<Controller>().Kill();
    }

    public void ChangeLevels(int newLevel)
    {
        //This should also be used if the player dies to reset the level
        switch (newLevel)
        {
            case 1:
                staticCamera = true;
                _camera._levelBounds = bounds1;
                break;
            case 2:
                staticCamera = true;
                _camera._levelBounds = bounds2;
                break;
            case 3:
                staticCamera = true;
                _camera._levelBounds = bounds3;
                break;
            case 4:
                staticCamera = true;
                _camera._levelBounds = bounds4;
                break;
            case 5:
                staticCamera = false;
                _camera._levelBounds = bounds5;
                break;
            case 6:
                _camera._levelBounds = bounds6;
                break;
            case 7:
            default:
                UnityEngine.SceneManagement.SceneManager.LoadScene("Ending Scene");
                break;
        }
        currentLevel = newLevel;
        if (currentLevel < 7)
        {
            player.transform.position = playerSpawns[currentLevel - 1].transform.position;

            ResetGameState?.Invoke();
        }
    }
}