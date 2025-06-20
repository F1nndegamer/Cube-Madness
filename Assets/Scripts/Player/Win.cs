using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Win : MonoBehaviour
{

    private GameObject Wintext;
    public PlayerRollingMovement[] players;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        players = FindObjectsByType<PlayerRollingMovement>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PlayerRollingMovement player in players)
        {
            if (!player.GameEnd)
            {
                return;
            }
        }
        if (Wintext != null)
        {
            Wintext.SetActive(true);
        }
        else
        {
            Wintext = GameManager.Instance.wintext;
            StartCoroutine(Winning());
        }
    }
    private IEnumerator Winning()
    {
        Wintext.SetActive(true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameManager.Instance.highestLevel = Mathf.Max(GameManager.Instance.highestLevel, SceneManager.GetActiveScene().buildIndex + 1);
        PlayerPrefs.SetInt("highestLevel", GameManager.Instance.highestLevel);
    }
}
