using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -5);
    public float smoothSpeed = 5f;
    public float rotationSpeed = 2f;

    private Quaternion targetRotation;
    [HideInInspector] public bool didstart;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        targetRotation = Quaternion.Euler(00f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        PlayerRollingMovement[] players = FindObjectsByType<PlayerRollingMovement>(FindObjectsSortMode.None);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].isActive) target = players[i].transform;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            didstart = true;
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                didstart = true;
            }
        }
    }
    private void LateUpdate()
    {
        if (target == null) return;

        if (didstart)
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
