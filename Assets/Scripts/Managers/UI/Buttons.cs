using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class Buttons : MonoBehaviour
{
    [SerializeField] public bool pause = false;
    [SerializeField] private GameObject pauseMenu;

    public void Restart()
    {
        if (CameraFollow.Instance.didstart == false) return;
        Reset reset = FindAnyObjectByType<Reset>();
        reset.ResetLevel();
    }
    public void Pause()
    {
        pause = !pause;
        Time.timeScale = pause ? 0 : 1;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(pause);
        }
        else
        {
            Debug.LogError("PauseMenu GameObject reference not set in the Inspector!");
        }
    }
    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }
}