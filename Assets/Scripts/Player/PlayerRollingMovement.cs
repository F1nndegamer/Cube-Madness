using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class PlayerRollingMovement : MonoBehaviour
{
    public float rollSpeed = 1f;
    public float fallSpeed = 5f;
    public float tileSize = 1f;
    public LayerMask groundLayer;
    public LayerMask EndLayer;
    public LayerMask PlayerLayer;
    private bool isRolling = false; 
    private GameObject Wintext;
    public bool isActive;
    public bool isFound;
    public Material activemat;
    void Start()
    {
        Wintext = GameManager.Instance.wintext;
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
    private void Update()
{
    if (isRolling) return;

    Vector3 direction = Vector3.zero;
    Vector3 up2 = new Vector3(0, 2, 0);
    if (Input.GetKeyDown(KeyCode.W)) direction = Vector3.forward;
    if (Input.GetKeyDown(KeyCode.S)) direction = Vector3.back;
    if (Input.GetKeyDown(KeyCode.A)) direction = Vector3.left;
    if (Input.GetKeyDown(KeyCode.D)) direction = Vector3.right;

    if (direction != Vector3.zero)
    {
        Vector3 targetPos = transform.position + direction * tileSize;

        Collider[] colliders = Physics.OverlapBox(targetPos, new Vector3(0.4f, 0.4f, 0.4f), Quaternion.identity, PlayerLayer);
        foreach (Collider col in colliders)
        {
            PlayerRollingMovement otherPlayer = col.GetComponent<PlayerRollingMovement>();
            if (otherPlayer != null)
            {
                 otherPlayer.Activate();
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
            StartCoroutine(RollAndFall(direction));
        }
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
        Vector3 pivot = transform.position + (direction * tileSize / 2f) + Vector3.down * (cubeSize / 2f);
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        for (float i = 0; i < 90; i += rollSpeed)
        {
            transform.RotateAround(pivot, rotationAxis, rollSpeed);
            yield return null;
        }

        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            Mathf.Round(transform.position.z)
        );

        isRolling = false;
    }

    private IEnumerator Fall()
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
        Wintext.SetActive(true);
        Debug.Log("Player Won level");
        isRolling = false;
    }
    public void Activate()
    {
        MeshRenderer meshrenderer = GetComponent<MeshRenderer>();
        meshrenderer.material = activemat;
        isActive = true;
        Switcher.Instance.playerlist.Add(this);
        isFound = true;
    }
}