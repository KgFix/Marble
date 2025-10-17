using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // prevent duplicates
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // keep playing between scenes
        }
    }
}
