using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private Button btnOn;
    [SerializeField] private Button btnOff;
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private Color inactiveColor = Color.white;

    private AudioManager audioManager;

    private void Start()
    {
        // Get a reference to the AudioManager
        audioManager = FindObjectOfType<AudioManager>();

        // Add listeners to the On and Off buttons
        btnOn.onClick.AddListener(() => SetSound(true));
        btnOff.onClick.AddListener(() => SetSound(false));

        // Initialize the button colors based on the sound state
        UpdateButtonsColor(IsSoundOn());
    }

    public void OpenOptions()
    {
        panelOptions.SetActive(true);
        Time.timeScale = 0;
    }

    public void CloseOptions()
    {
        panelOptions.SetActive(false);
        Time.timeScale = 1;
    }

    public void RemoveUI()
    {
        menuUI.SetActive(false);
    }

    public void DisplayUI()
    {
        menuUI.SetActive(true);
    }

    // Public function to enable or disable sound (called by the buttons)
    public void SetSound(bool isOn)
    {
        if (audioManager != null)
        {
            audioManager.SetSound(isOn); // Call the SetSound function of the AudioManager
            UpdateButtonsColor(isOn);      // Update the button colors
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
    }

    // Updates the color of the buttons based on the sound state
    private void UpdateButtonsColor(bool isOn)
    {
        btnOn.GetComponent<Image>().color = isOn ? activeColor : inactiveColor;
        btnOff.GetComponent<Image>().color = !isOn ? activeColor : inactiveColor;
    }

    // Checks if the sound is enabled or not by accessing the AudioManager's state
    private bool IsSoundOn()
    {
        if (audioManager != null)
        {
            // Access the isMuted variable of the AudioManager (make sure it exists and is public)
            return !audioManager.isMuted; // Invert the isMuted state to get the "sound enabled" state
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
            return false;
        }
    }
}
