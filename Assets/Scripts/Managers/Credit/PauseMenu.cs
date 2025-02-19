using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour

{

    [SerializeField] GameObject pauseMenu;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Home()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;

        /*
        // Restart the music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeMusic(); // Resume existing music
        }
        else
        {
            Debug.LogError("AudioManager instance not found!");
        }
        */
    }
/*

        public void PauseMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PauseMusic();
        }
        else
        {
            Debug.LogError("AudioManager instance not found!");
        }
    }

    public void ResumeMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeMusic();
        }
        else
        {
            Debug.LogError("AudioManager instance not found!");
        }
    }

*/
}
