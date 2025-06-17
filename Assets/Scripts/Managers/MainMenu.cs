using System;
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
}

