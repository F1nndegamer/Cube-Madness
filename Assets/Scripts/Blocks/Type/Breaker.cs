using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BreakableBlock : MonoBehaviour
{
    public List<Transform> blocks;
    private List<Transform> previousBlocks = new List<Transform>();
    public Material brokenMaterial;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BreakBlocks();
        }
    }

    private void BreakBlocks()
    {
        foreach (Transform block in blocks)
        {
            Destroy(block.gameObject);
            Destroy(this.gameObject);
        }
    }

    private void OnValidate()
    {
        foreach (Transform block in blocks)
        {
            if (block != null && !previousBlocks.Contains(block))
            {
                MeshRenderer blockmeshRenderer = block.GetComponent<MeshRenderer>();
                if (blockmeshRenderer != null)
                    blockmeshRenderer.material = brokenMaterial;
            }
        }
        previousBlocks = new List<Transform>(blocks);
    }
}