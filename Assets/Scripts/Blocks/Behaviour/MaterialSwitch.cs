using UnityEngine;

public class MaterialSwitch : MonoBehaviour
{
    private Material runtime;
    public Material dark,
        light;

    public void Start()
    {
        Switch();
    }

    public void Switch()
    {
        GetComponent<Renderer>().materials[0] = Global.darkMode ? dark : light;
    }
}
