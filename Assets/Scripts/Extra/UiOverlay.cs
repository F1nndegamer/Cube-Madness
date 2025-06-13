using System.Data.Common;
using TMPro;
using UnityEngine;

public class UIDebugOverlayTMP : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    public GameObject ddolCanvas;
    float deltaTime = 0f;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

#if DEVELOPMENT_BUILD
        debugText.gameObject.SetActive(true);
        DontDestroyOnLoad(ddolCanvas);
#else
        DontDestroyOnLoad(ddolCanvas);
        debugText.gameObject.SetActive(false);
#endif
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1f / deltaTime;

        debugText.text = $"FPS: {fps:F1}";
        debugText.color = (fps < 30f) ? Color.red : Color.white;
    }
}
