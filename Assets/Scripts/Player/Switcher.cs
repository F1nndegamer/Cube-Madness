using UnityEngine;
using System.Collections.Generic;

public class Switcher : MonoBehaviour
{
    public List<PlayerRollingMovement> playerlist = new List<PlayerRollingMovement>();
    public int currentIndex = 0;
    public static Switcher Instance;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
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
        if (CameraFollow.Instance.didstart)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    float distance = Vector2.Distance(touchStartPos, touch.position);

                    if (distance < 10f)
                    {
                        if (playerlist.Count == 0 || !playerlist[currentIndex].isActive || playerlist[currentIndex].isRolling == false)
                        {
                            SwitchToNextPlayer();
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchToNextPlayer();
            }

        }
    }



    private void SwitchToNextPlayer()
    {
        if (playerlist.Count == 0) return;
        playerlist[currentIndex].enabled = false;
        currentIndex = (currentIndex + 1) % playerlist.Count;
        playerlist[currentIndex].enabled = true;
        CameraFollow.Instance.target = playerlist[currentIndex].transform;
    }

    private void SwitchToPreviousPlayer()
    {
        if (playerlist.Count == 0) return;
        playerlist[currentIndex].enabled = false;
        currentIndex--;
        if (currentIndex < 0) currentIndex = playerlist.Count - 1;
        playerlist[currentIndex].enabled = true;
        CameraFollow.Instance.target = playerlist[currentIndex].transform;
    }
}
