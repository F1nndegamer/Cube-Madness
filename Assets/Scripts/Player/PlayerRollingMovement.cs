using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class PlayerRollingMovement : MonoBehaviour
{
    public float rollSpeed = 5f;
    public float fallSpeed = 5f;
    public float tileSize = 1f;
    public LayerMask groundLayer;
    public LayerMask EndLayer;
    public LayerMask PlayerLayer;
    public LayerMask WallLayer;
    public bool isRolling = false;
    public bool isActive;
    public bool isFound;
    public Material activemat;
    public bool GameEnd;
    void Start()
    {

        FallManager.Instance.RegisterPlayer(this);
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Ground");
        }
        if (EndLayer == 0)
        {
            EndLayer = LayerMask.GetMask("End");
        }
        if (PlayerLayer == 0)
        {
            PlayerLayer = LayerMask.GetMask("Player");
        }
    }
    void OnDestroy()
    {
        FallManager.Instance.UnregisterPlayer(this);
    }
    private void Update()
    {
        if (isRolling) return;

        bool onGround = Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, 1f, groundLayer);
        bool onEnd = Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, 1f, EndLayer);

        if (!onGround && !onEnd)
        {
            StartCoroutine(Fall());
            return;
        }

        Vector3 direction = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W)) direction = Vector3.forward;
        if (Input.GetKeyDown(KeyCode.S)) direction = Vector3.back;
        if (Input.GetKeyDown(KeyCode.A)) direction = Vector3.left;
        if (Input.GetKeyDown(KeyCode.D)) direction = Vector3.right;

        if (direction == Vector3.zero) return;


        Vector3 targetPos = transform.position + direction * tileSize;
        Collider[] colliders = Physics.OverlapBox(targetPos, new Vector3(0.4f, 0.4f, 0.4f), Quaternion.identity, PlayerLayer);
        Vector3 wallPos = targetPos - direction * (tileSize * 0.5f);
        Collider[] wallcolliders = Physics.OverlapBox(wallPos, new Vector3(0.4f, 0.4f, 0.4f), Quaternion.identity, WallLayer);
        if(wallcolliders.Length > 0)
        {
            
        }
        else if (colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                PlayerRollingMovement otherPlayer = col.GetComponent<PlayerRollingMovement>();
                if (otherPlayer != null)
                {
                    otherPlayer.Activate();
                    return;
                }
            }
        }

        else if (Physics.Raycast(targetPos + Vector3.up * 0.5f, Vector3.down, 1f, groundLayer))
        {
            StartCoroutine(Roll(direction));
        }
        else if (Physics.Raycast(targetPos + Vector3.up * 0.5f, Vector3.down, 1f, EndLayer))
        {
            StartCoroutine(RollAndEnd(direction));
        }
        else
        {
            StartCoroutine(RollAndFall(direction));
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

    private IEnumerator Roll(Vector3 direction)
    {
        isRolling = true;
        float cubeSize = GetComponent<Collider>().bounds.size.y;
        Vector3 pivot = transform.position + (direction * tileSize / 2f) + Vector3.down * ((float)0.9 / 2f);
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


        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            (float)0.95,
            Mathf.Round(transform.position.z)
        );

        isRolling = false;
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
        float targetposition = transform.position.y - tileSize;
        while (transform.position.y > targetposition)
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
        MeshRenderer meshrenderer = GetComponent<MeshRenderer>();
        meshrenderer.material = activemat;
        isActive = true;
        if (!Switcher.Instance.playerlist.Contains(this))
        {
            Switcher.Instance.playerlist.Add(this);
        }
        isFound = true;
    }

}