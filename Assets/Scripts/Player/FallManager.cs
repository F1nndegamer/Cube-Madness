using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallManager : MonoBehaviour
{
    public static FallManager Instance;
    public List<PlayerRollingMovement> players = new List<PlayerRollingMovement>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        foreach (PlayerRollingMovement player in players)
        {
            if (player == null || !player.gameObject.activeInHierarchy || player.isRolling) continue;

            bool onGround = Physics.Raycast(player.transform.position + Vector3.up * 0.5f, Vector3.down, 1f, player.groundLayer);
            bool onEnd = Physics.Raycast(player.transform.position + Vector3.up * 0.5f, Vector3.down, 1f, player.EndLayer);

            if (!onGround && !onEnd && !player.isMovingObject)
            {
                StartCoroutine(player.Fall());
            }
        }
    }

    public void RegisterPlayer(PlayerRollingMovement player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }

    public void UnregisterPlayer(PlayerRollingMovement player)
    {
        players.Remove(player);
    }
}
