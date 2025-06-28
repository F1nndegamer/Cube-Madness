using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject wintext;
    public static GameManager Instance;
    public int Levels = 8;
    public int highestLevelNormal = 0;
    public int highestLevelTime = 0;
    public int highestLevelMoves = 0;
    void Awake()
    {
        Instance = this;
        highestLevelNormal = PlayerPrefs.GetInt("highestLevelNormal", highestLevelNormal);
        highestLevelTime = PlayerPrefs.GetInt("highestLevelTime", highestLevelTime);
        highestLevelMoves = PlayerPrefs.GetInt("highestLevelMoves", highestLevelMoves);
    }
    void Update()
    {
                
    }
}
