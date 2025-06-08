using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 10f;
    public float speed = 20f;
    public bool isAnimated = true;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        if (target == null || !isAnimated) return;

        transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);

        Vector3 direction = (transform.position - target.position).normalized;
        transform.position = target.position + direction * distance;

        transform.LookAt(target);
    }

    public void Switch(bool animated)
    {
        isAnimated = animated;
        if (!isAnimated)
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }
    }
}
