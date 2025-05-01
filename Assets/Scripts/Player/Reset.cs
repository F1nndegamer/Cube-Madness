using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Reset : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
