using UnityEngine;
using System.Collections.Generic;

public class Switcher : MonoBehaviour
{
    public List<PlayerRollingMovement> playerlist = new List<PlayerRollingMovement>();
    public int currentIndex = 0;
    public static Switcher Instance;
    void Start()
    {
        Instance = this;
        PlayerRollingMovement[] players = FindObjectsByType<PlayerRollingMovement>(FindObjectsSortMode.None);
        foreach (PlayerRollingMovement player in players)
        {
            if (player.isFound)
            {
                playerlist.Add(player);
            }
        }
    }

    void Update()

    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (playerlist.Count == 0) return;
            playerlist[currentIndex].enabled = false;
            currentIndex = (currentIndex + 1) % playerlist.Count;
            playerlist[currentIndex].enabled = true;
            CameraFollow.Instance.target = playerlist[currentIndex].transform;
        }
    }
}
