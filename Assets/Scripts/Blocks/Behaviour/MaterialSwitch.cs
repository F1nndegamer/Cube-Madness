using UnityEngine;

public class MaterialSwitch : MonoBehaviour
{
    private Material runtime;
    public Material dark, light;

    public void Start()
    {
        Switch();
    }

    public void Switch()
    {
        GetComponent<Renderer>().material = Global.darkMode ? dark : light;
        Debug.Log(Global.darkMode ? "Switched to dark mode" : "Switched to light mode");
    }
}
