using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRollingMovement : MonoBehaviour
{
    public float rollSpeed = 5f;
    public float fallSpeed = 5f;
    public float tileSize = 1f;

    public LayerMask groundLayer;
    public LayerMask EndLayer;
    public LayerMask PlayerLayer;
    public LayerMask WallLayer;
    public LayerMask MoveLayer;

    public bool isRolling = false;
    public bool isActive;
    public bool isFound;
    public bool GameEnd;

    public Material activemat;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private float minSwipeDist = 50f;
    private float maxTapTime = 0.2f;
    private float touchStartTime;

    private Vector3? bufferedDirection = null;
    private float bufferDuration = 0.1f;
    private float bufferTimer = 0f;
    private Vector3 SavedDir;
    public bool isMovingObject;
    [SerializeField] public bool died;
    void Start()
    {
        FallManager.Instance.RegisterPlayer(this);

        if (groundLayer == 0)
            groundLayer = LayerMask.GetMask("Ground");
        if (EndLayer == 0)
            EndLayer = LayerMask.GetMask("End");
        if (PlayerLayer == 0)
            PlayerLayer = LayerMask.GetMask("Player");
        if (WallLayer == 0)
            WallLayer = LayerMask.GetMask("Wall");
        if (MoveLayer == 0)
            MoveLayer = LayerMask.GetMask("Movable");
    }

    void OnDestroy()
    {
        FallManager.Instance.UnregisterPlayer(this);
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (Mode.Instance.MovesLeft < 0 && Mode.Instance.currentMode == "Moves")
        {
            if (!died)
            {
                died = true;
                Die();
            }
            return;
        }
        if (bufferTimer > 0f)
            bufferTimer -= Time.deltaTime;
        else
            bufferedDirection = null;


        Vector3 direction = Vector3.zero;
        if (SavedDir != Vector3.zero) { direction = SavedDir; Debug.Log("completed"); }
        SavedDir = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W)) direction = Vector3.forward;
        if (Input.GetKeyDown(KeyCode.S)) direction = Vector3.back;
        if (Input.GetKeyDown(KeyCode.A)) direction = Vector3.left;
        if (Input.GetKeyDown(KeyCode.D)) direction = Vector3.right;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    touchStartTime = Time.time;
                    break;

                case TouchPhase.Ended:
                    touchEndPos = touch.position;
                    float swipeDist = (touchEndPos - touchStartPos).magnitude;
                    float swipeTime = Time.time - touchStartTime;

                    if (swipeDist >= minSwipeDist)
                    {
                        Vector2 swipeVector = touchEndPos - touchStartPos;
                        swipeVector.Normalize();

                        if (Vector2.Dot(swipeVector, Vector2.up) > 0.7f)
                            direction = Vector3.forward;
                        else if (Vector2.Dot(swipeVector, Vector2.down) > 0.7f)
                            direction = Vector3.back;
                        else if (Vector2.Dot(swipeVector, Vector2.left) > 0.7f)
                            direction = Vector3.left;
                        else if (Vector2.Dot(swipeVector, Vector2.right) > 0.7f)
                            direction = Vector3.right;
                    }
                    else if (swipeTime <= maxTapTime)
                    {
                        if (!isActive)
                        {
                            Activate();
                        }
                    }
                    break;
            }
        }

        if (isRolling)
        {
            if (direction != Vector3.zero)
            {
                bufferedDirection = direction;
                bufferTimer = bufferDuration;
                Debug.Log("saved");
            }
            return;
        }

        if (direction == Vector3.zero) return;

        Vector3 targetPos = transform.position + direction * tileSize;
        Vector3 wallPos = targetPos - direction * (tileSize * 0.5f);

        Collider[] wallColliders = Physics.OverlapBox(wallPos, new Vector3(0.4f, 0.4f, 0.4f), Quaternion.identity, WallLayer);
        if (wallColliders.Length > 0)
        {
            return;
        }
        Collider[] playerColliders = Physics.OverlapBox(targetPos, new Vector3(0.4f, 0.4f, 0.4f), Quaternion.identity, PlayerLayer);
        if (playerColliders.Length > 0)
        {
            foreach (Collider col in playerColliders)
            {
                PlayerRollingMovement otherPlayer = col.GetComponent<PlayerRollingMovement>();
                if (otherPlayer != null)
                {
                    otherPlayer.Activate();
                    return;
                }
            }
        }

        Collider[] moveColliders = Physics.OverlapBox(targetPos, new Vector3(0.4f, 0.4f, 0.4f), Quaternion.identity, MoveLayer);
        if (moveColliders.Length > 0)
        {
            foreach (Collider col in moveColliders)
            {
                Movable movable = col.GetComponent<Movable>();
                if (movable != null)
                {
                    if (!movable.Check(direction)) return;
                    movable.Move(direction);
                    StartCoroutine(Roll(direction));
                    return;
                }
            }
        }

        if (Physics.Raycast(targetPos + Vector3.up * 0.5f, Vector3.down, 1f, groundLayer))
        {
            StartCoroutine(Roll(direction));
        }
        else if (Physics.Raycast(targetPos + Vector3.up * 0.5f, Vector3.down, 1f, EndLayer))
        {
            StartCoroutine(RollAndEnd(direction));
        }
        else
        {
            return;
        }
    }

    private IEnumerator Roll(Vector3 direction)
    {
        Mode.Instance.Moves++;
        isRolling = true;
        float cubeSize = GetComponent<Collider>().bounds.size.y;
        Vector3 pivot = transform.position + (direction * tileSize / 2f) + Vector3.down * (0.9f / 2f);
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        float rotatedAmount = 0f;
        while (rotatedAmount < 90f)
        {
            float step = rollSpeed * Time.deltaTime * 60f;
            transform.RotateAround(pivot, rotationAxis, step);
            rotatedAmount += step;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(
            Mathf.Round(transform.rotation.eulerAngles.x / 90) * 90,
            Mathf.Round(transform.rotation.eulerAngles.y / 90) * 90,
            Mathf.Round(transform.rotation.eulerAngles.z / 90) * 90
        );
        float position = Mathf.Round(transform.position.y) - 0.05f;
        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            position,
            Mathf.Round(transform.position.z)
        );

        isRolling = false;

        if (bufferedDirection.HasValue && bufferTimer > 0f)
        {
            Vector3 bufferedDir = bufferedDirection.Value;
            bufferedDirection = null;
            bufferTimer = 0f;

            yield return null;

            SavedDir = bufferedDir;
            Update();
        }

    }

    private IEnumerator RollAndFall(Vector3 direction)
    {
        yield return StartCoroutine(Roll(direction));
        StartCoroutine(Fall());
    }

    private IEnumerator RollAndEnd(Vector3 direction)
    {
        yield return StartCoroutine(Roll(direction));
        StartCoroutine(End());
    }

    public IEnumerator Fall()
    {
        isRolling = true;

        while (transform.position.y > -5f)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        Debug.Log("Player fell");
        isRolling = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator End()
    {
        isRolling = true;
        float targetY = transform.position.y - tileSize;

        while (transform.position.y > targetY)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        GameEnd = true;
        isRolling = false;

        Switcher.Instance.playerlist.Remove(this);

        if (Switcher.Instance.playerlist.Count > 0)
        {
            Switcher.Instance.currentIndex %= Switcher.Instance.playerlist.Count;
            Switcher.Instance.playerlist[Switcher.Instance.currentIndex].enabled = true;
            CameraFollow.Instance.target = Switcher.Instance.playerlist[Switcher.Instance.currentIndex].transform;
        }

        gameObject.SetActive(false);
    }

    public void Activate()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = activemat;
        isActive = true;

        if (!Switcher.Instance.playerlist.Contains(this))
        {
            Switcher.Instance.playerlist.Add(this);
        }

        isFound = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {

        }
    }
    public void Die()
    {
        Debug.Log("Player died");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
