using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Tutorial : MonoBehaviour
{
    private String[] Tutorials;
    public TextMeshProUGUI tutorialText;
    string keybind = "";
    string MoveKey = "";
    string ResetButton = "";
    void Start()
    {
#if UNITY_ANDROID
        MoveKey = " Swipe left/right/up/down ";
        keybind = "Tap the screen ";
        ResetButton = " the reset button ";
#else
        MoveKey = " Use WASD "; 
        keybind = "Use Tab ";
        ResetButton = " R ";
#endif
        Tutorials = new String[]
        {
            "Welcome to the game!" + MoveKey + "to move to the end.",
            keybind + "to switch active players.",
            "Press" + ResetButton + "to reset the level if you get stuck.",
            " ",
            "You can move the new block by using the player",
        };

        for (int i = 0; i < Tutorials.Length; i++)
        {
            if (i == SceneManager.GetActiveScene().buildIndex - 1)
            {
                tutorialText.text = Tutorials[i];
                break;

            }
        }
    }

    void Update()
    {

    }
}
