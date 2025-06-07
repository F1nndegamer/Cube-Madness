using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject wintext;
    public GameObject tap;
    public static GameManager Instance;
    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        if (CameraFollow.Instance.didstart)
        {
            tap.SetActive(false);
        }
    }
}
