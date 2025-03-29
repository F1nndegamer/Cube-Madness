using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExtraTileTrigger : MonoBehaviour
{
    public List<Transform> objectToMove;
    public List<Vector3> targetPosition = new List<Vector3>();
    private List<Vector3> Location = new List<Vector3>();
    public float moveSpeed = 2f;
    public bool needs_active = true;

    [SerializeField] private MovementOrder movementOrder = MovementOrder.AtOnce; // SerializeField to expose enum in the inspector

    public enum MovementOrder
    {
        AtOnce,
        XFirst,
        YFirst,
        ZFirst
    }

    private void Start()
    {
        if (targetPosition.Count != objectToMove.Count)
        {
            Debug.LogWarning("targetPosition count does not match objectToMove count.");
        }

        for (int i = 0; i < objectToMove.Count; i++)
        {
            Location.Add(objectToMove[i].position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(MoveObject());
            print("PlayerEnter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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

        bool moving = true;
        while (moving)
        {
            moving = false;

            switch (movementOrder)
            {
                case MovementOrder.AtOnce:
                    for (int i = 0; i < objectToMove.Count; i++)
                    {
                        if (i >= targetPosition.Count) continue;

                        Transform obj = objectToMove[i];
                        Vector3 targetPos = targetPosition[i];

                        if (Vector3.Distance(obj.position, targetPos) > 0f)
                        {
                            obj.position = Vector3.MoveTowards(obj.position, targetPos, moveSpeed * Time.deltaTime);
                            moving = true;
                        }
                    }
                    break;

                case MovementOrder.XFirst:
                    moving = MoveInOrder(0);
                    break;

                case MovementOrder.YFirst:
                    moving = MoveInOrder(1);
                    break;

                case MovementOrder.ZFirst:
                    moving = MoveInOrder(2);
                    break;
            }

            yield return null;
        }
    }

    private IEnumerator MoveObjectBack()
    {
        bool moving = true;
        while (moving)
        {
            moving = false;

            switch (movementOrder)
            {
                case MovementOrder.AtOnce:
                    for (int i = 0; i < objectToMove.Count; i++)
                    {
                        Transform obj = objectToMove[i];
                        Vector3 startPos = Location[i];

                        if (Vector3.Distance(obj.position, startPos) > 0f)
                        {
                            obj.position = Vector3.MoveTowards(obj.position, startPos, moveSpeed * Time.deltaTime);
                            moving = true;
                        }
                    }
                    break;

                case MovementOrder.XFirst:
                    moving = MoveBackInOrder(0);
                    break;

                case MovementOrder.YFirst:
                    moving = MoveBackInOrder(1);
                    break;

                case MovementOrder.ZFirst:
                    moving = MoveBackInOrder(2);
                    break;
            }

            yield return null;
        }

        if (needs_active)
        {
            foreach (Transform obj in objectToMove)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    private bool MoveInOrder(int axis)
    {
        bool moving = false;

        for (int i = 0; i < objectToMove.Count; i++)
        {
            if (i >= targetPosition.Count) continue;

            Transform obj = objectToMove[i];
            Vector3 targetPos = targetPosition[i];
            Vector3 currentPos = obj.position;

            switch (axis)
            {
                case 0:
                    if (currentPos.x != targetPos.x)
                    {
                        currentPos.x = Mathf.MoveTowards(currentPos.x, targetPos.x, moveSpeed * Time.deltaTime);
                        moving = true;
                    }
                    break;
                case 1:
                    if (currentPos.y != targetPos.y)
                    {
                        currentPos.y = Mathf.MoveTowards(currentPos.y, targetPos.y, moveSpeed * Time.deltaTime);
                        moving = true;
                    }
                    break;
                case 2:
                    if (currentPos.z != targetPos.z)
                    {
                        currentPos.z = Mathf.MoveTowards(currentPos.z, targetPos.z, moveSpeed * Time.deltaTime);
                        moving = true;
                    }
                    break;
            }

            obj.position = currentPos;
        }

        return moving;
    }

    private bool MoveBackInOrder(int axis)
    {
        bool moving = false;

        for (int i = 0; i < objectToMove.Count; i++)
        {
            Transform obj = objectToMove[i];
            Vector3 startPos = Location[i];
            Vector3 currentPos = obj.position;

            switch (axis)
            {
                case 0:
                    if (currentPos.x != startPos.x)
                    {
                        currentPos.x = Mathf.MoveTowards(currentPos.x, startPos.x, moveSpeed * Time.deltaTime);
                        moving = true;
                    }
                    break;
                case 1:
                    if (currentPos.y != startPos.y)
                    {
                        currentPos.y = Mathf.MoveTowards(currentPos.y, startPos.y, moveSpeed * Time.deltaTime);
                        moving = true;
                    }
                    break;
                case 2:
                    if (currentPos.z != startPos.z)
                    {
                        currentPos.z = Mathf.MoveTowards(currentPos.z, startPos.z, moveSpeed * Time.deltaTime);
                        moving = true;
                    }
                    break;
            }

            obj.position = currentPos;
        }

        return moving;
    }
}
