using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movable : MonoBehaviour
{
    public float moveTime = 0.2f;
    public float tileSize = 1f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask playerLayer;
    public bool canMoveUp;
    public bool canMoveDown;
    public bool canMoveLeft;
    public bool canMoveRight;

    private void Update()
    {
        CheckAllDirections();
    }
    public bool Check(Vector3 direction)
    {
        if (direction == Vector3.forward) return canMoveUp;
        if (direction == Vector3.back) return canMoveDown;
        if (direction == Vector3.left) return canMoveLeft;
        if (direction == Vector3.right) return canMoveRight;
        return false;
    }


    private void CheckAllDirections()
    {
        canMoveUp = CanMove(Vector3.forward);
        canMoveDown = CanMove(Vector3.back);
        canMoveLeft = CanMove(Vector3.left);
        canMoveRight = CanMove(Vector3.right);
    }

    private bool CanMove(Vector3 direction)
    {
        Vector3 targetPos = transform.position + direction * tileSize;
        Vector3 playerPos = transform.position + direction * tileSize;
        Vector3 wallCheckPos = targetPos - direction * (tileSize * 0.5f);

        Collider[] wallColliders = Physics.OverlapBox(wallCheckPos, new Vector3(0.4f, 0.4f, 0.4f), Quaternion.identity, wallLayer);
        if (wallColliders.Length > 0)
        {
            return false;
        }
        Collider[] playerColliders = Physics.OverlapBox(playerPos, new Vector3(0.4f, 0.4f, 0.4f), Quaternion.identity, playerLayer);
        if (playerColliders.Length > 0)
        {
            return false;
        }
        return Physics.Raycast(targetPos + Vector3.up * 0.5f, Vector3.down, 1f, groundLayer);
    }

    public void Move(Vector3 direction)
    {
        if (!CanMove(direction))
            return;

        Vector3 targetPos = transform.position + direction * tileSize;
        StartCoroutine(MoveSmooth(targetPos));
    }

    private IEnumerator MoveSmooth(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }
}
