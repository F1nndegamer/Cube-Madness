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
    string[] MovementOrder = { "At Once", "X First", "Y First", "Z First" };
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
            for (int i = 0; i < objectToMove.Count; i++)
            {
                if (i >= targetPosition.Count) continue; // Prevent out-of-range errors

                Transform obj = objectToMove[i];
                Vector3 targetPos = targetPosition[i];

                if (Vector3.Distance(obj.position, targetPos) > 0f)
                {
                    obj.position = Vector3.MoveTowards(obj.position, targetPos, moveSpeed * Time.deltaTime);
                    moving = true;
                }
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
}
