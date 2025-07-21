using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Mode : MonoBehaviour
{
    public string currentMode = "Normal";
    public static Mode Instance;
    public TextMeshProUGUI modeText;

    [Header("Time")]

    [SerializeField] public float TimeLeft = 0f;
    float TimeElapsed = 0f;
    float[] MaxTime = { 3f, 8f, 11f, 15f, 8f, 20f, 20f, 17f, 0f, 0f };

    [Header("Moves")]

    public int Moves;
    public int MovesLeft = 0;
    int[] MaxMoves = { 5, 17, 23, 36, 16, 37, 25, 37, 0, 0 };

    void Start()
    {
        currentMode = PlayerPrefs.GetString("GameMode", "Normal");
        Instance = this;
    }

    void Update()
    {
        modeText.transform.parent.gameObject.SetActive(true);
        if (currentMode == "Normal")
        {
            modeText.transform.parent.gameObject.SetActive(false);
            return;
        }
        else if (currentMode == "Time")
        {
            int currentlevel = SceneManager.GetActiveScene().buildIndex - 1;
            if (!CameraFollow.Instance.didstart) { modeText.text = "Time Left: " + MaxTime[currentlevel]; return; }
            TimeElapsed += Time.deltaTime;
            TimeLeft = MaxTime[currentlevel] - TimeElapsed;
            if (TimeLeft <= 0f)
            {
                TimeLeft = 0f;
            }
            modeText.enabled = true;
            if (MaxTime[currentlevel] > 0) modeText.text = "Time Left: " + TimeLeft.ToString("F2") + "s";
            else modeText.text = "Time Left: " + TimeElapsed.ToString("F2") + "s";
        }
        else if (currentMode == "Moves")
        {
            modeText.enabled = true;
            int currentlevel = SceneManager.GetActiveScene().buildIndex - 1;
            MovesLeft = MaxMoves[currentlevel] - Moves;
            modeText.text = "Moves Left: " + MovesLeft.ToString();
        }

    }
}
