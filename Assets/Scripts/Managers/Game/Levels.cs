using UnityEngine;

public class LevelsPanel : MonoBehaviour
{
    public int levels;
    public static int levelsUnlocked = 1;
    [SerializeField] private GameObject levelParent;

    void Start()
    {
        levels = GameManager.Instance.Levels;
        if (levelsUnlocked == 0) levelsUnlocked = 1;
    }

    public void Open()
    {
        foreach (Transform child in levelParent.transform)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Generating level buttons for " + levels + " levels.");
        string currentmode = PlayerPrefs.GetString("GameMode", "Normal");
        levelsUnlocked = PlayerPrefs.GetInt("highestLevel" + currentmode);
        if (levelsUnlocked == 0) { levelsUnlocked = 1; }
        GameObject buttonPrefab = Resources.Load<GameObject>("LevelButton");
        if (buttonPrefab == null)
        {
            Debug.LogError("LevelButton prefab not found in Resources folder.");
            return;
        }

        for (int i = 1; i <= levels; i++)
        {
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
