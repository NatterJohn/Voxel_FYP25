using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private Voxel[,,] voxels;
    private int chunkSize = 16;
    private Color gizmoColor;

    void Start()
    {
        voxels = new Voxel[chunkSize, chunkSize, chunkSize];
        InitializeVoxels();

    }

    private void InitializeVoxels()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    voxels[x, y, z] = new Voxel(transform.position + new Vector3(x, y, z), Color.white);
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        if (voxels != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(transform.position + new Vector3(chunkSize / 2, chunkSize / 2, chunkSize / 2), new Vector3(chunkSize, chunkSize, chunkSize));
        }
    }
    public void Initialize(int size)
    {
        this.chunkSize = size;
        voxels = new Voxel[size, size, size];
        InitializeVoxels();
        gizmoColor = new Color(Random.value, Random.value, Random.value, 0.4f); // Semi-transparent

    }
}
