using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject wintext;
    public static GameManager Instance;
    public int Levels = 7;
    public int highestLevel = 0;
    void Awake()
    {
        Instance = this;
        highestLevel = PlayerPrefs.GetInt("highestLevel", highestLevel);
    }
    void Update()
    {

    }
}
