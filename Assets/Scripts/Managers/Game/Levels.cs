using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelsPanel : MonoBehaviour
{
    bool opened = false;
    public int levels;
    public static int levelsUnlocked = 1;
    [SerializeField] private GameObject levelParent;

    void Start()
    {
        levelsUnlocked = PlayerPrefs.GetInt("highestLevel");
        levels = GameManager.Instance.Levels;
        if (levelsUnlocked == 0) { levelsUnlocked = 1; }
    }

    public void Open()
    {
        if (!opened)
        {
            opened = true;
            levelsUnlocked = PlayerPrefs.GetInt("highestLevel");
            if (levelsUnlocked == 0) { levelsUnlocked = 1; }

            for (int i = 1; i <= levels; i++)
            {
                GameObject buttonPrefab = Resources.Load<GameObject>("LevelButton");
                if (buttonPrefab == null)
                {
                    Debug.LogError("LevelButton prefab not found in Resources folder.");
                    continue;
                }
                LevelButton button = Instantiate(buttonPrefab, levelParent.transform).GetComponent<LevelButton>();
                if (i <= levelsUnlocked)
                {
                    button.unlocked = true;
                }
                else
                {
                    button.unlocked = false;
                }
                button.n = i;
                button.buttonUpdate();
            }
        }
    }
}
