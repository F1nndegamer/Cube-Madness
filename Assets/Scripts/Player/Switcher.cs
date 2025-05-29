using UnityEngine;
using System.Collections.Generic;

public class Switcher : MonoBehaviour
{
    public List<PlayerRollingMovement> playerlist = new List<PlayerRollingMovement>();
    public int currentIndex = 0;
    public static Switcher Instance;

    // Touch detection for switching players
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private float minSwipeDist = 50f;

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
        // Keyboard cycling (Tab)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchToNextPlayer();
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;

                case TouchPhase.Ended:
                    touchEndPos = touch.position;
                    float swipeDist = (touchEndPos - touchStartPos).magnitude;

                    if (swipeDist >= minSwipeDist)
                    {
                        Vector2 swipeVector = touchEndPos - touchStartPos;
                        swipeVector.Normalize();

                        if (Vector2.Dot(swipeVector, Vector2.left) > 0.7f)
                        {
                            SwitchToPreviousPlayer();
                        }
                        else if (Vector2.Dot(swipeVector, Vector2.right) > 0.7f)
                        {
                            SwitchToNextPlayer();
                        }
                    }
                    break;
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
