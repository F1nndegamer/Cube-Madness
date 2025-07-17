using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #region Buttons
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Options() { }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
    #endregion
    #region FPS
    public TextMeshProUGUI fpsText;
    public Slider fpsSlider;
    public Toggle unlimitedFPSToggle;
    bool unlimitedFPS = false;
    int targetFPS = 60;

    public void SetFPS(Single fps)
    {
        targetFPS = (int)fps;
        if (unlimitedFPS)
        {
            Debug.LogWarning("Cannot set FPS while unlimited FPS is enabled.");
            return;
        }
        Application.targetFrameRate = (int)fps;
        Debug.Log("FPS set to: " + fps);
        fpsText.text = "FPS: " + fps.ToString("F0");
    }

    public void ToggleUnlimitedFPS(bool enable)
    {
        unlimitedFPS = enable;
        if (unlimitedFPS)
        {
            Application.targetFrameRate = -1; // Set to unlimited
            Debug.Log("Unlimited FPS enabled.");
            fpsText.text = "FPS: Unlimited";
        }
        else
        {
            Application.targetFrameRate = targetFPS; // Default FPS
            Debug.Log("Unlimited FPS disabled. FPS set to: " + targetFPS);
            fpsText.text = "FPS: " + targetFPS.ToString("F0");
        }
    }
    #endregion
    #region ModeSelect
    public string modes = "Normal";
    public string username;
    public Image modenormal;
    public Image modetime;
    public Image modefewestmoves;

    public void SetUsername(string name)
    {
        username = name;
        Debug.Log("Username set to: " + name);
    }

    public void SelectMode(string mode)
    {
        switch (mode)
        {
            case "Normal":
                Debug.Log("Normal mode selected");
                modes = "Normal";
                if (modenormal == null || modetime == null || modefewestmoves == null)
                    break;
                modenormal.enabled = true;
                modetime.enabled = false;
                modefewestmoves.enabled = false;
                break;
            case "Fewest Moves":
                Debug.Log("Fewest Moves mode selected");
                modes = "Moves";
                if (modenormal == null || modetime == null || modefewestmoves == null)
                    break;
                modenormal.enabled = false;
                modetime.enabled = false;
                modefewestmoves.enabled = true;
                break;
            case "Time Trial":
                Debug.Log("Time Trial mode selected");
                modes = "Time";
                if (modenormal == null || modetime == null || modefewestmoves == null)
                    break;
                modenormal.enabled = false;
                modetime.enabled = true;
                modefewestmoves.enabled = false;
                break;
        }
    }
    #endregion
    #region ThemeSwitcher
    public GameObject BG;

    public void SetTheme(bool dark)
    {
        Global.darkMode = dark;
        MaterialSwitch[] switches = BG.GetComponentsInChildren<MaterialSwitch>();
        for (int i = 0; i < switches.Length; i++)
        {
            switches[i].Switch();
        }
    }
    #endregion
    #region SaveLoad
    public TMP_InputField usernameText;

    private void Awake()
    {
        LoadGame();
        Debug.Log(
            "Game loaded with mode: " + modes + ", FPS: " + targetFPS + ", Username: " + username
        );
        if (usernameText != null)
            usernameText.text = username;
        if (fpsText != null)
        {
            fpsText.text = unlimitedFPS ? "FPS: Unlimited" : "FPS: " + targetFPS.ToString("F0");
        }
        if (fpsSlider != null)
            fpsSlider.value = targetFPS;
        if (unlimitedFPSToggle != null)
            unlimitedFPSToggle.isOn = unlimitedFPS;
        if (modes == "Normal")
            SelectMode("Normal");
        else if (modes == "Moves")
            SelectMode("Fewest Moves");
        else if (modes == "Time")
            SelectMode("Time Trial");
        else
            SelectMode("Normal");
        SetFPS(targetFPS);
        SelectMode(modes);
    }

    public void SaveGame()
    {
        PlayerPrefs.SetString("GameMode", modes);
        PlayerPrefs.SetInt("TargetFPS", targetFPS);
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.SetInt("UnlimitedFPS", unlimitedFPS ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Game saved");
    }

    public void LoadGame()
    {
        modes = PlayerPrefs.GetString("GameMode", "Normal");
        targetFPS = PlayerPrefs.GetInt("TargetFPS", 60);
        username = PlayerPrefs.GetString("Username", "Player");
        unlimitedFPS = PlayerPrefs.GetInt("UnlimitedFPS", 0) == 1;
        Debug.Log("Game loaded");
    }
    #endregion
}
