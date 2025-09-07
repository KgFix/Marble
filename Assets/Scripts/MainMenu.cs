using UnityEngine;

public class MainMenu : MonoBehaviour
{
    //Load Level Scene
    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelMenu");
    }

    public void OpenOptions()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("OptionsMenu");
    }

    //Quit Application
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUIT!");
    }

}
