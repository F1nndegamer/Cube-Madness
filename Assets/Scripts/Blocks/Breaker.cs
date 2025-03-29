using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BreakableBlock : MonoBehaviour
{
    public List<Transform> blocks;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BreakBlocks();
        }
    }

    private void BreakBlocks()
    {
       foreach(Transform block in blocks)
       {
            Destroy(block.gameObject);
            Destroy(this.gameObject);
       }
    }
}
