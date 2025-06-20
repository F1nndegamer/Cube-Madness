using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class LevelButton : MonoBehaviour
{
    public int n = 0;
    public bool unlocked = false;
    [SerializeField] TextMeshProUGUI levelText;

    void Start()
    {
        StartCoroutine(ButtonIn());
    }

    private IEnumerator ButtonIn()
    {
        yield return new WaitForSecondsRealtime(n / 10f);
        while (transform.localScale.x < 1)
        {
            transform.localScale += Vector3.one * Time.deltaTime * 5;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }
    public void buttonUpdate()
    {
        levelText.text = "Level " + n.ToString();
        GetComponent<Button>().interactable = unlocked;
    }
    public void PlayLevel()
    {
        SceneManager.LoadScene("Level" + n.ToString());
    }
}
