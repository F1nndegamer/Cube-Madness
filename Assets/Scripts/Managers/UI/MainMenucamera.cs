using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public Transform targetfield;
    public float distance = 10f;
    public float speed = 20f;
    public bool isAnimated = true;

    public float targetMoveSpeed = 0.1f;

    public float minY = 0f;
    public float maxY = 10f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Vector3 lastMousePosition;
    private bool isDragging = false;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        Switch(true);
    }

    void Update()
    {
        if (target == null || !isAnimated) return;

        HandleDrag();

        transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);

        Vector3 direction = (transform.position - target.position).normalized;
        transform.position = target.position + direction * distance;

        transform.LookAt(target);

    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            float moveY = mouseDelta.y * targetMoveSpeed;
            Vector3 targetPos = transform.position;
            targetPos.y = Mathf.Clamp(targetPos.y + moveY, minY, maxY);
            transform.position = targetPos;
        }
    }

    public void Switch(bool animated)
    {
        isAnimated = animated;
        if (!isAnimated && transform.position != startPosition || !isAnimated &&
        transform.rotation != startRotation)
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }
    }
}
