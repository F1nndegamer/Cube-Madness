using UnityEngine;
using System.Collections;
public class ExtraTileTrigger : MonoBehaviour
{
    public Transform objectToMove;
    public Vector3 targetPosition;
    private Vector3 Location;
    public float moveSpeed = 2f;
    public bool needs_active = true;

    private void Start()
    {
        Location = objectToMove.position;
    }
    private void OnTriggerEnter(Collider other)
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
        if(needs_active)
        {
            objectToMove.gameObject.SetActive(true);
        }
        while (Vector3.Distance(objectToMove.position, targetPosition) > 0.01f)
        {
            objectToMove.position = Vector3.MoveTowards(objectToMove.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
    private IEnumerator MoveObjectBack()
    {
        
        while (Vector3.Distance(objectToMove.position, Location) > 0.01f)
        {
            objectToMove.position = Vector3.MoveTowards(objectToMove.position, Location, moveSpeed * Time.deltaTime);
            yield return null;
        }
        if(needs_active)
        {
            objectToMove.gameObject.SetActive(false);
        }
    }
}
