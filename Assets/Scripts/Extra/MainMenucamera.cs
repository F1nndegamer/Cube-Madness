using UnityEngine;
using System.Collections;

public class CubeTumbler : MonoBehaviour
{
    public float rotationDuration = 0.3f;
    public float delayBetweenRotations = 0.2f;

    private bool isTumbling = false;

    void Start()
    {
        StartCoroutine(TumbleSequence());
    }

    IEnumerator TumbleSequence()
    {
        isTumbling = true;

        Vector3[] directions = {
            Vector3.right,
            Vector3.forward,
            Vector3.left,
            Vector3.back
        };

        foreach (Vector3 dir in directions)
        {
            yield return StartCoroutine(Tumble(dir));
            yield return new WaitForSeconds(delayBetweenRotations);
        }

        yield return StartCoroutine(RotateToTop());

        isTumbling = false;
    }

    IEnumerator Tumble(Vector3 direction)
    {
        Vector3 anchor = transform.position + (Vector3.down + direction) * 0.5f;
        Vector3 axis = Vector3.Cross(Vector3.up, direction);

        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.AngleAxis(90f, axis) * startRotation;

        while (elapsed < rotationDuration)
        {
            transform.RotateAround(anchor, axis, (90f / rotationDuration) * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
        transform.position = RoundToGrid(transform.position);
    }

    IEnumerator RotateToTop()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.identity;
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
    }

    Vector3 RoundToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x),
            Mathf.Round(pos.y),
            Mathf.Round(pos.z)
        );
    }
}
