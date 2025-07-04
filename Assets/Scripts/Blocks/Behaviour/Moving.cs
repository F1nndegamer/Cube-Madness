using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

    private void Awake()
    {
        foreach (Transform obj in objectToMove)
        {
            obj.gameObject.isStatic = false;
        }
    }

    private void Start()
    {
        players = FindObjectsByType<PlayerRollingMovement>(FindObjectsSortMode.None);
        if (targetPosition.Count != objectToMove.Count)
        {
            Debug.LogWarning("targetPosition count does not match objectToMove count.");
        }

        foreach (Transform obj in objectToMove)
        {
            Location.Add(obj.position);
            if (!obj.TryGetComponent(out ObjectMoving _))
                obj.gameObject.AddComponent<ObjectMoving>();
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

    private bool TryClaimOwnership(Transform obj, bool moveToTarget)
    {
        var state = obj.GetComponent<ObjectMoving>();
        if (state == null) return true;

        if (state.currentOwner == null)
        {
            state.currentOwner = this;
            state.isMovingToTarget = moveToTarget;
            return true;
        }

        if (state.currentOwner == this)
        {
            state.isMovingToTarget = moveToTarget;
            return true;
        }

        if (!state.isMovingToTarget && moveToTarget)
        {
            state.currentOwner = this;
            state.isMovingToTarget = true;
            return true;
        }

        return false;
    }

    private void ReleaseOwnership(Transform obj)
    {
        var state = obj.GetComponent<ObjectMoving>();
        if (state != null && state.currentOwner == this)
        {
            state.currentOwner = null;
        }
    }

    private void OnDisable()
    {
        foreach (Transform obj in objectToMove)
        {
            ReleaseOwnership(obj);
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
                    Vector3 expectedPos = objectToMove[i].position + new Vector3(0, PlayerHeight, 0);
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

                if (!TryClaimOwnership(obj, true)) continue;

                switch (Order)
                {
                    case MovementOrder.At_Once:
                        if (Vector3.Distance(obj.position, targetPos) > 0.01f)
                        {
                            obj.position = Vector3.MoveTowards(obj.position, targetPos, moveSpeed * Time.deltaTime);
                            isMoving = true;
                        }
                        else
                        {
                            ReleaseOwnership(obj);
                        }
                        break;

                    case MovementOrder.X_First:
                        if (Mathf.Abs(obj.position.x - targetPos.x) > 0.01f)
                        {
                            obj.position = new Vector3(
                                Mathf.MoveTowards(obj.position.x, targetPos.x, moveSpeed * Time.deltaTime),
                                obj.position.y,
                                obj.position.z
                            );
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.y - targetPos.y) > 0.01f || Mathf.Abs(obj.position.z - targetPos.z) > 0.01f)
                        {
                            obj.position = new Vector3(
                                obj.position.x,
                                Mathf.MoveTowards(obj.position.y, targetPos.y, moveSpeed * Time.deltaTime),
                                Mathf.MoveTowards(obj.position.z, targetPos.z, moveSpeed * Time.deltaTime)
                            );
                            isMoving = true;
                        }
                        else
                        {
                            ReleaseOwnership(obj);
                        }
                        break;

                    case MovementOrder.Y_First:
                        if (Mathf.Abs(obj.position.y - targetPos.y) > 0.01f)
                        {
                            obj.position = new Vector3(
                                obj.position.x,
                                Mathf.MoveTowards(obj.position.y, targetPos.y, moveSpeed * Time.deltaTime),
                                obj.position.z
                            );
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.x - targetPos.x) > 0.01f || Mathf.Abs(obj.position.z - targetPos.z) > 0.01f)
                        {
                            obj.position = new Vector3(
                                Mathf.MoveTowards(obj.position.x, targetPos.x, moveSpeed * Time.deltaTime),
                                obj.position.y,
                                Mathf.MoveTowards(obj.position.z, targetPos.z, moveSpeed * Time.deltaTime)
                            );
                            isMoving = true;
                        }
                        else
                        {
                            ReleaseOwnership(obj);
                        }
                        break;

                    case MovementOrder.Z_First:
                        if (Mathf.Abs(obj.position.z - targetPos.z) > 0.01f)
                        {
                            obj.position = new Vector3(
                                obj.position.x,
                                obj.position.y,
                                Mathf.MoveTowards(obj.position.z, targetPos.z, moveSpeed * Time.deltaTime)
                            );
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.x - targetPos.x) > 0.01f || Mathf.Abs(obj.position.y - targetPos.y) > 0.01f)
                        {
                            obj.position = new Vector3(
                                Mathf.MoveTowards(obj.position.x, targetPos.x, moveSpeed * Time.deltaTime),
                                Mathf.MoveTowards(obj.position.y, targetPos.y, moveSpeed * Time.deltaTime),
                                obj.position.z
                            );
                            isMoving = true;
                        }
                        else
                        {
                            ReleaseOwnership(obj);
                        }
                        break;
                }
            }

            for (int p = 0; p < players.Length; p++)
            {
                int objIndex = playeronobject[p];
                if (objIndex >= 0 && objIndex < objectToMove.Count)
                {
                    players[p].isMovingObject = isMoving;
                    players[p].transform.position = objectToMove[objIndex].position + new Vector3(0, PlayerHeight, 0);
                }
            }

            yield return new WaitForFixedUpdate();
        }

        for (int p = 0; p < players.Length; p++)
        {
            players[p].isMovingObject = false;
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
                    Vector3 expectedPos = objectToMove[i].position + new Vector3(0, PlayerHeight, 0);
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
                Vector3 targetPos = Location[i];
                Transform obj = objectToMove[i];

                if (!TryClaimOwnership(obj, false)) continue;

                switch (Order)
                {
                    case MovementOrder.At_Once:
                        if (Vector3.Distance(obj.position, targetPos) > 0.01f)
                        {
                            obj.position = Vector3.MoveTowards(obj.position, targetPos, moveSpeed * Time.deltaTime);
                            isMoving = true;
                        }
                        else
                        {
                            ReleaseOwnership(obj);
                        }
                        break;

                    case MovementOrder.X_First:
                        if (Mathf.Abs(obj.position.z - targetPos.z) > 0.01f)
                        {
                            obj.position = new Vector3(obj.position.x, obj.position.y, Mathf.MoveTowards(obj.position.z, targetPos.z, moveSpeed * Time.deltaTime));
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.y - targetPos.y) > 0.01f)
                        {
                            obj.position = new Vector3(obj.position.x, Mathf.MoveTowards(obj.position.y, targetPos.y, moveSpeed * Time.deltaTime), obj.position.z);
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.x - targetPos.x) > 0.01f)
                        {
                            obj.position = new Vector3(Mathf.MoveTowards(obj.position.x, targetPos.x, moveSpeed * Time.deltaTime), obj.position.y, obj.position.z);
                            isMoving = true;
                        }
                        else
                        {
                            ReleaseOwnership(obj);
                        }
                        break;

                    case MovementOrder.Y_First:
                        if (Mathf.Abs(obj.position.z - targetPos.z) > 0.01f)
                        {
                            obj.position = new Vector3(obj.position.x, obj.position.y, Mathf.MoveTowards(obj.position.z, targetPos.z, moveSpeed * Time.deltaTime));
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.x - targetPos.x) > 0.01f)
                        {
                            obj.position = new Vector3(Mathf.MoveTowards(obj.position.x, targetPos.x, moveSpeed * Time.deltaTime), obj.position.y, obj.position.z);
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.y - targetPos.y) > 0.01f)
                        {
                            obj.position = new Vector3(obj.position.x, Mathf.MoveTowards(obj.position.y, targetPos.y, moveSpeed * Time.deltaTime), obj.position.z);
                            isMoving = true;
                        }
                        else
                        {
                            ReleaseOwnership(obj);
                        }
                        break;

                    case MovementOrder.Z_First:
                        if (Mathf.Abs(obj.position.x - targetPos.x) > 0.01f)
                        {
                            obj.position = new Vector3(Mathf.MoveTowards(obj.position.x, targetPos.x, moveSpeed * Time.deltaTime), obj.position.y, obj.position.z);
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.y - targetPos.y) > 0.01f)
                        {
                            obj.position = new Vector3(obj.position.x, Mathf.MoveTowards(obj.position.y, targetPos.y, moveSpeed * Time.deltaTime), obj.position.z);
                            isMoving = true;
                        }
                        else if (Mathf.Abs(obj.position.z - targetPos.z) > 0.01f)
                        {
                            obj.position = new Vector3(obj.position.x, obj.position.y, Mathf.MoveTowards(obj.position.z, targetPos.z, moveSpeed * Time.deltaTime));
                            isMoving = true;
                        }
                        else
                        {
                            ReleaseOwnership(obj);
                        }
                        break;
                }
            }

            for (int p = 0; p < players.Length; p++)
            {
                int objIndex = playeronobject[p];
                if (objIndex >= 0 && objIndex < objectToMove.Count)
                {
                    players[p].isMovingObject = isMoving;
                    players[p].transform.position = objectToMove[objIndex].position + new Vector3(0, PlayerHeight, 0);
                }
            }

            yield return new WaitForFixedUpdate();
        }

        for (int p = 0; p < players.Length; p++)
        {
            players[p].isMovingObject = false;
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
