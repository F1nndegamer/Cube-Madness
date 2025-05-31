using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExtraTileTrigger : MonoBehaviour
{
    public List<Transform> objectToMove;
    public List<Vector3> targetPosition = new List<Vector3>();
    private List<Vector3> Location = new List<Vector3>();
    public PlayerRollingMovement[] players;
    public List<int> playeronobject;
    public float moveSpeed = 2f;
    public bool needs_active = true;
    public enum MovementOrder { At_Once, X_First, Y_First, Z_First };
    public MovementOrder Order;
    public bool isMoving;
    public bool playermoves;
    public float PlayerHeight = 0.95f;

    private void Start()
    {
        players = FindObjectsByType<PlayerRollingMovement>(FindObjectsSortMode.None);
        if (targetPosition.Count != objectToMove.Count)
        {
            Debug.LogWarning("targetPosition count does not match objectToMove count.");
        }

        for (int i = 0; i < objectToMove.Count; i++)
        {
            Location.Add(objectToMove[i].position);
        }
        playeronobject = new List<int>(new int[players.Length]);
        for (int i = 0; i < playeronobject.Count; i++)
        {
            playeronobject[i] = -1;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isMoving)
        {
            StopAllCoroutines();
            StartCoroutine(MoveObject());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(MoveObjectBack());
        }
    }

    private IEnumerator MoveObject()
    {
        if (needs_active)
        {
            foreach (Transform obj in objectToMove)
            {
                obj.gameObject.SetActive(true);
            }
        }

        isMoving = true;

        while (isMoving)
        {
            for (int p = 0; p < players.Length; p++)
            {
                PlayerRollingMovement player = players[p];
                playeronobject[p] = -1;
                for (int i = 0; i < objectToMove.Count; i++)
                {
                    Transform block = objectToMove[i];
                    Vector3 expectedPos = block.position + new Vector3(0, PlayerHeight, 0);
                    if (player.transform.position == expectedPos)
                    {
                        playeronobject[p] = i;
                        break;
                    }
                }
            }

            isMoving = false;
            for (int i = 0; i < objectToMove.Count; i++)
            {
                if (i >= targetPosition.Count) continue;

                Transform obj = objectToMove[i];
                Vector3 targetPos = targetPosition[i];
                switch (Order)
                {
                    case MovementOrder.At_Once:
                        if (Vector3.Distance(obj.position, targetPos) > 0f)
                        {
                            obj.position = Vector3.MoveTowards(obj.position, targetPos, moveSpeed * Time.deltaTime);
                            isMoving = true;
                        }
                        break;
                    case MovementOrder.X_First:
                        if (Mathf.Abs(obj.position.x - targetPos.x) > 0f)
                        {
                            obj.position = new Vector3(
                                Mathf.MoveTowards(obj.position.x, targetPos.x, moveSpeed * Time.deltaTime),
                                obj.position.y,
                                obj.position.z
                            );
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.y - targetPos.y) > 0f || Mathf.Abs(obj.position.z - targetPos.z) > 0f)
                        {
                            obj.position = new Vector3(
                                obj.position.x,
                                Mathf.MoveTowards(obj.position.y, targetPos.y, moveSpeed * Time.deltaTime),
                                Mathf.MoveTowards(obj.position.z, targetPos.z, moveSpeed * Time.deltaTime)
                            );
                            isMoving = true;
                        }
                        break;
                    case MovementOrder.Y_First:
                        if (Mathf.Abs(obj.position.y - targetPos.y) > 0f)
                        {
                            obj.position = new Vector3(
                                obj.position.x,
                                Mathf.MoveTowards(obj.position.y, targetPos.y, moveSpeed * Time.deltaTime),
                                obj.position.z
                            );
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.x - targetPos.x) > 0f || Mathf.Abs(obj.position.z - targetPos.z) > 0f)
                        {
                            obj.position = new Vector3(
                                Mathf.MoveTowards(obj.position.x, targetPos.x, moveSpeed * Time.deltaTime),
                                obj.position.y,
                                Mathf.MoveTowards(obj.position.z, targetPos.z, moveSpeed * Time.deltaTime)
                            );
                            isMoving = true;
                        }
                        break;
                    case MovementOrder.Z_First:
                        if (Mathf.Abs(obj.position.z - targetPos.z) > 0f)
                        {
                            obj.position = new Vector3(
                                obj.position.x,
                                obj.position.y,
                                Mathf.MoveTowards(obj.position.z, targetPos.z, moveSpeed * Time.deltaTime)
                            );
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.x - targetPos.x) > 0f || Mathf.Abs(obj.position.y - targetPos.y) > 0f)
                        {
                            obj.position = new Vector3(
                                Mathf.MoveTowards(obj.position.x, targetPos.x, moveSpeed * Time.deltaTime),
                                Mathf.MoveTowards(obj.position.y, targetPos.y, moveSpeed * Time.deltaTime),
                                obj.position.z
                            );
                            isMoving = true;
                        }
                        break;
                }
            }

            for (int p = 0; p < players.Length; p++)
            {
                int objIndex = playeronobject[p];
                if (objIndex >= 0 && objIndex < objectToMove.Count)
                {
                    players[p].transform.position = objectToMove[objIndex].position + new Vector3(0, PlayerHeight, 0);
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator MoveObjectBack()
    {
        isMoving = true;

        while (isMoving)
        {
            for (int p = 0; p < players.Length; p++)
            {
                PlayerRollingMovement player = players[p];
                playeronobject[p] = -1;
                for (int i = 0; i < objectToMove.Count; i++)
                {
                    Transform block = objectToMove[i];
                    Vector3 expectedPos = block.position + new Vector3(0, PlayerHeight, 0);
                    if (player.transform.position == expectedPos)
                    {
                        playeronobject[p] = i;
                        break;
                    }
                }
            }

            isMoving = false;
            for (int i = 0; i < objectToMove.Count; i++)
            {
                Transform obj = objectToMove[i];
                Vector3 targetPos = Location[i];

                switch (Order)
                {
                    case MovementOrder.At_Once:
                        if (Vector3.Distance(obj.position, targetPos) > 0f)
                        {
                            obj.position = Vector3.MoveTowards(obj.position, targetPos, moveSpeed * Time.deltaTime);
                            isMoving = true;
                        }
                        break;
                    case MovementOrder.X_First:
                        if (Mathf.Abs(obj.position.z - targetPos.z) > 0f)
                        {
                            obj.position = new Vector3(
                                obj.position.x,
                                obj.position.y,
                                Mathf.MoveTowards(obj.position.z, targetPos.z, moveSpeed * Time.deltaTime)
                            );
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.y - targetPos.y) > 0f)
                        {
                            obj.position = new Vector3(
                                obj.position.x,
                                Mathf.MoveTowards(obj.position.y, targetPos.y, moveSpeed * Time.deltaTime),
                                obj.position.z
                            );
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.x - targetPos.x) > 0f)
                        {
                            obj.position = new Vector3(
                                Mathf.MoveTowards(obj.position.x, targetPos.x, moveSpeed * Time.deltaTime),
                                obj.position.y,
                                obj.position.z
                            );
                            isMoving = true;
                        }
                        break;
                    case MovementOrder.Y_First:
                        if (Mathf.Abs(obj.position.z - targetPos.z) > 0f)
                        {
                            obj.position = new Vector3(
                                obj.position.x,
                                obj.position.y,
                                Mathf.MoveTowards(obj.position.z, targetPos.z, moveSpeed * Time.deltaTime)
                            );
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.x - targetPos.x) > 0f)
                        {
                            obj.position = new Vector3(
                                Mathf.MoveTowards(obj.position.x, targetPos.x, moveSpeed * Time.deltaTime),
                                obj.position.y,
                                obj.position.z
                            );
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.y - targetPos.y) > 0f)
                        {
                            obj.position = new Vector3(
                                obj.position.x,
                                Mathf.MoveTowards(obj.position.y, targetPos.y, moveSpeed * Time.deltaTime),
                                obj.position.z
                            );
                            isMoving = true;
                        }
                        break;
                    case MovementOrder.Z_First:
                        if (Mathf.Abs(obj.position.x - targetPos.x) > 0f)
                        {
                            obj.position = new Vector3(
                                Mathf.MoveTowards(obj.position.x, targetPos.x, moveSpeed * Time.deltaTime),
                                obj.position.y,
                                obj.position.z
                            );
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.y - targetPos.y) > 0f)
                        {
                            obj.position = new Vector3(
                                obj.position.x,
                                Mathf.MoveTowards(obj.position.y, targetPos.y, moveSpeed * Time.deltaTime),
                                obj.position.z
                            );
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.z - targetPos.z) > 0f)
                        {
                            obj.position = new Vector3(
                                obj.position.x,
                                obj.position.y,
                                Mathf.MoveTowards(obj.position.z, targetPos.z, moveSpeed * Time.deltaTime)
                            );
                            isMoving = true;
                        }
                        break;
                }
            }

            for (int p = 0; p < players.Length; p++)
            {
                int objIndex = playeronobject[p];
                if (objIndex >= 0 && objIndex < objectToMove.Count)
                {
                    players[p].transform.position = objectToMove[objIndex].position + new Vector3(0, PlayerHeight, 0);
                }
            }

            yield return new WaitForFixedUpdate();
        }

        if (needs_active)
        {
            foreach (Transform obj in objectToMove)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }
}
