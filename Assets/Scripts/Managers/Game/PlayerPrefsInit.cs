using System.Data;
using UnityEngine;

public class PlayerPrefsInit : MonoBehaviour
{
    void Awake()
    {
        if (!PlayerPrefs.HasKey("highestLevelNormal"))
        {
            PlayerPrefs.SetInt("highestLevelNormal", 0);
        }
        if (PlayerPrefs.HasKey("highestLevel"))
        {
            int highestLevel = PlayerPrefs.GetInt("highestLevel");
            PlayerPrefs.SetInt("highestLevelNormal", highestLevel);
            PlayerPrefs.DeleteKey("highestLevel");
        }
        if (!PlayerPrefs.HasKey("highestLevelTime"))
        {
            PlayerPrefs.SetInt("highestLevelTime", 0);
        }
        if (!PlayerPrefs.HasKey("highestLevelMoves"))
        {
            PlayerPrefs.SetInt("highestLevelMoves", 0);
        }
        if (!PlayerPrefs.HasKey("GameMode"))
        {
            PlayerPrefs.SetString("GameMode", "Normal");
        }
        Destroy(this);
    }
}
