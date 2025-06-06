using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = true;
    }

    void Update()
    {
        
    }

    public void ChangeLevel(string levelName)
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }
}
