using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayText : MonoBehaviour
{
    public GameObject tap;
    void Start()
    {
#if UNITY_ANDROID
        tap.GetComponent<TextMeshProUGUI>().text = "Tap to Start";
#else
        tap.GetComponent<TextMeshProUGUI>().text = "Press Space to Start";
#endif

    }

    void Update()
    {
        if (CameraFollow.Instance.didstart)
        {
            tap.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
}
