using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
        StartCoroutine(Gamestart());
    }
    IEnumerator Gamestart()
    {
        tap.SetActive(true);
        Time.timeScale = 0f;
        yield return new WaitUntil(() => CameraFollow.Instance.didstart);
        tap.SetActive(false);
        Time.timeScale = 1f;
    }
}
