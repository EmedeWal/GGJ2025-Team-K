using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour
{
    public static event Action resetGameState;

    [SerializeField] private Camera camera;
    [SerializeField] private CinemachineCamera cineCamera;
    private Controller player;

    public int currentLevel = 1;
    public bool staticCamera = true;

    [SerializeField] private List<GameObject> playerSpawns;

    void Start()
    {
        camera = FindAnyObjectByType<Camera>();
        cineCamera = FindAnyObjectByType<CinemachineCamera>();
        player = FindAnyObjectByType<Controller>();
        changeLevels(1);
    }

    private void OnDisable()
    {
        resetGameState = null;
    }

    private void Update()
    {
        //DEBUG TOOLS to go to the next or previous level
        if (Input.GetKeyDown(KeyCode.O))
        {
            changeLevels(currentLevel + 1);
            Debug.Log("Current Level + 1");
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            changeLevels(currentLevel - 1);
            Debug.Log("Current Level - 1");
        }
    }

    public void changeLevels(int newLevel)
    {
        //This should also be used if the player dies to reset the level
        switch (newLevel)
        {
            case 1:
                staticCamera = true;
                cineCamera.enabled = false;
                camera.transform.position = new Vector3(0.48f, 0, camera.transform.position.z);
                break;
            case 2:
                staticCamera = true;
                cineCamera.enabled = false;
                camera.transform.position = new Vector3(36f, 0, camera.transform.position.z);
                break;
            case 3:
                staticCamera = true;
                cineCamera.enabled = false;
                camera.transform.position = new Vector3(70f, 0, camera.transform.position.z);
                break;
            case 4:
                staticCamera = false; //Should start at 105f
                cineCamera.enabled = true;
                break;
            default:
                break;
        }
        currentLevel = newLevel;
        player.transform.position = playerSpawns[currentLevel - 1].transform.position;
    }
}
