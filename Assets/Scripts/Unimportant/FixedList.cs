using UnityEngine;
using System.Collections.Generic;

public class AxisStringOrder : MonoBehaviour
{
    public List<string> axes = new List<string> { "x", "y", "z" };

    private void OnValidate()
    {
        if (axes.Count != 3)
        {
            axes = new List<string> { "x", "y", "z" };
        }
    }
}
