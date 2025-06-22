using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Buttons : MonoBehaviour
{
    bool pause = false;
    [SerializeField] private GameObject pauseMenu;

    public void Restart()
    {
        if (CameraFollow.Instance.didstart == false) return;
        Reset reset = FindAnyObjectByType<Reset>();
        reset.ResetLevel();
    }
    public void Pause()
    {
        if (CameraFollow.Instance.didstart == false) return;
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }
}