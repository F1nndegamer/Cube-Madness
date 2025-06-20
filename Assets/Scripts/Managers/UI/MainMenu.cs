using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    #region Buttons
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Options()
    {

    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
    #endregion
    #region FPS
    public TextMeshProUGUI fpsText;
    bool unlimitedFPS = false;
    int targetFPS = 60;
    void Start()
    {
        Application.targetFrameRate = 120;
    }
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
        unlimitedFPS = !enable;
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
                break;
            case "Fewest Moves":
                Debug.Log("Fewest Moves mode selected");
                modes = "Fewest Moves";
                break;
            case "Time Trial":
                Debug.Log("Time Trial mode selected");
                modes = "Time Trial";
                break;
        }


    }
    //TODO: add functionality to the mode selection, by having level selection based on this and player preferences, also add settings to player preferences
    #endregion
    #region SaveLoad
    public void SaveGame()
    {
        PlayerPrefs.SetString("GameMode", modes);
        PlayerPrefs.SetInt("TargetFPS", targetFPS);
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.Save();
        Debug.Log("Game saved");
    }
    public void LoadGame()
    {
        modes = PlayerPrefs.GetString("GameMode", "Normal");
        targetFPS = PlayerPrefs.GetInt("TargetFPS", 60);
        username = PlayerPrefs.GetString("Username", "Player");
        Debug.Log("Game loaded");
    }
    #endregion
}

