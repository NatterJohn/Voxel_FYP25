using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MarchingSpheres : MonoBehaviour
{
    public Material sphereMaterial;

    private Container container;
    [SerializeField] private int width = 25;
    [SerializeField] private int height = 26;
    [SerializeField] float resolution = 1;

    [SerializeField] private float heightTresshold = 0.5f;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private float[,,] heights;

    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        StartCoroutine(TestAll());
    }

    void Update()
    {

    }

    private IEnumerator TestAll()
    {
        while (true)
        {
            generateVoxels();
            MarchCubes();
            SetMesh();
            yield return new WaitForSeconds(1f);
        }
    }

    private void SetMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
    
    private void generateVoxels()
    {
        heights = new float[width + 1, height + 1, width + 1];

        // Sphere parameters
        GameObject sphereContainer = new GameObject("Container");
        sphereContainer.transform.parent = transform;
        container = sphereContainer.AddComponent<Container>();
        container.Initialize(sphereMaterial, Vector3.zero);
        float radiusSphere = 8.5f;
        Vector3 centerSphere = new Vector3(radiusSphere, radiusSphere, radiusSphere);

        for (int x = 0; x < 17; x++)
        {
            for (int y = 0; y < 17; y++)
            {
                for (int z = 0; z < 17; z++)
                {
                    Vector3 positionOfVoxel = new Vector3(x, y, z);
                    float distanceFromCenter = Vector3.Distance(positionOfVoxel, centerSphere);
                    if (distanceFromCenter <= radiusSphere)
                    {
                        container[positionOfVoxel] = new Voxel() { ID = 1 };
                    }
                }
            }
        }

        container.GenerateMesh();
        container.UploadMesh();
    }
    private int GetConfigIndex(float[] cubeCorners)
    {
        int configIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > heightTresshold)
            {
                configIndex |= 1 << i;
            }
        }

        return configIndex;
    }

    private void MarchCubes()
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    float[] cubeCorners = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), cubeCorners);
                }
            }
        }
    }

    private void MarchCube(Vector3 position, float[] cubeCorners)
    {
        int configIndex = GetConfigIndex(cubeCorners);

        if (configIndex == 0 || configIndex == 255)
        {
            return;
        }

        int edgeIndex = 0;
        for (int t = 0; t < 5; t++)
        {
            for (int v = 0; v < 3; v++)
            {
                int triTableValue = MarchingTable.Triangles[configIndex, edgeIndex];

                if (triTableValue == -1)
                {
                    return;
                }

                Vector3 edgeStart = position + MarchingTable.Edges[triTableValue, 0];
                Vector3 edgeEnd = position + MarchingTable.Edges[triTableValue, 1];

                Vector3 vertex = (edgeStart + edgeEnd) / 2;

                vertices.Add(vertex);
                triangles.Add(vertices.Count - 1);

                edgeIndex++;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        /*if (!visualizeNoise || !Application.isPlaying)
        {
            return;
        }
        */
        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z], 1);
                    Gizmos.DrawSphere(new Vector3(x * resolution, y * resolution, z * resolution), 0.2f * resolution);
                }
            }
        }
    }
}
