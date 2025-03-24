using UnityEngine;
using System.Collections.Generic;

public class Switcher : MonoBehaviour
{
    private PlayerRollingMovement[] players;
    private int currentIndex = 0;

    void Start()
    {
        players = Resources.FindObjectsOfTypeAll<PlayerRollingMovement>();

        for (int i = 0; i < players.Length; i++)
        {
            players[i].enabled = (i == 0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            players[currentIndex].enabled = false;
            currentIndex = (currentIndex + 1) % players.Length;
            players[currentIndex].enabled = true;
            CameraFollow.Instance.target = players[currentIndex].transform;
            Debug.Log("Switched to: " + players[currentIndex].gameObject.name);
        }
    }
}
