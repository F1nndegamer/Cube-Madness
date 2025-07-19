using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Win : MonoBehaviour
{
    [Header("UI References")]
    public GameObject winPanel;

    private GameObject winTextObject;
    private TextMeshProUGUI winText;
    private bool hasWon = false;

    private PlayerRollingMovement[] players;
    private List<bool> winstatus = new List<bool>();

    void Start()
    {
        players = FindObjectsByType<PlayerRollingMovement>(FindObjectsSortMode.None);
        winstatus = new List<bool>(new bool[players.Length]);
        if (GameManager.Instance != null)
        {
            winTextObject = GameManager.Instance.wintext;
            winText = winTextObject?.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (hasWon) return;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GameEnd)
            {
                winstatus[i] = true;
            }
        }
        if (!winstatus.Contains(false))
        {
            Debug.Log(string.Join(",", winstatus) + " " + players.Length);
            hasWon = true;
            winstatus.Clear();
            ShowWinScreen();
            StartCoroutine(HandleWin());

        }
    }

    private void ShowWinScreen()
    {
        Image winimage = winPanel.GetComponent<Image>();
        Color c = Camera.main.backgroundColor;
        c.a = 0f;
        winimage.color = c;

        if (winText == null && GameManager.Instance != null)
        {
            winTextObject = GameManager.Instance.wintext;
            winText = winTextObject?.GetComponent<TextMeshProUGUI>();
        }

        if (winText != null)
        {
            StartCoroutine(FadeTextAlpha(winText, winText.color.a, 1f, 1f));
        }

        if (winPanel != null)
        {
            var winImage = winPanel.GetComponent<Image>();
            if (winImage != null)
            {
                StartCoroutine(FadeImageAlpha(winImage, winImage.color.a, 1f, 1f));
            }
        }
    }

    private IEnumerator HandleWin()
    {
        yield return new WaitForSeconds(2f);

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);

        if (GameManager.Instance != null)
        {
            string currentMode = PlayerPrefs.GetString("GameMode", "Normal");
            if (currentMode == "Normal")
            {
                GameManager.Instance.highestLevelNormal = Mathf.Max(GameManager.Instance.highestLevelNormal, nextSceneIndex);
                PlayerPrefs.SetInt("highestLevelNormal", GameManager.Instance.highestLevelNormal);
            }
            else if (currentMode == "Time")
            {
                GameManager.Instance.highestLevelTime = Mathf.Max(GameManager.Instance.highestLevelTime, nextSceneIndex);
                PlayerPrefs.SetInt("highestLevelTime", GameManager.Instance.highestLevelTime);
            }
            else if (currentMode == "Moves")
            {
                GameManager.Instance.highestLevelMoves = Mathf.Max(GameManager.Instance.highestLevelMoves, nextSceneIndex);
                PlayerPrefs.SetInt("highestLevelMoves", GameManager.Instance.highestLevelMoves);
            }

        }
    }

    private IEnumerator FadeTextAlpha(TextMeshProUGUI text, float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = text.color;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(from, to, elapsed / duration);
            text.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = to;
        text.color = color;
    }

    private IEnumerator FadeImageAlpha(Image image, float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = image.color;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(from, to, elapsed / duration);
            image.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = to;
        image.color = color;
    }
}
