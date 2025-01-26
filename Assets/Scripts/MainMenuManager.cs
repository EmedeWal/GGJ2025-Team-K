using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ChangeLevel(string levelName)
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }
}
