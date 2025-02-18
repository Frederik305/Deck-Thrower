using UnityEngine;

public class InfoMenu : MonoBehaviour
{

    [SerializeField] GameObject PanelInfo;
    [SerializeField] GameObject MenuUI;

    public void Pause()
    {
        PanelInfo.SetActive(true);
        Time.timeScale = 0;
    }
    public void RemoveUI()
    {
        MenuUI.SetActive(false);
    }

    public void DisplayUI()
    {
        MenuUI.SetActive(true);
    }

    public void Resume()
    {
        PanelInfo.SetActive(false);
        Time.timeScale = 1;
    }
}

