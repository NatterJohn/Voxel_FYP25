using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SphereMaker : MonoBehaviour
{
    public Material sphereMaterial;

    private Container container;


    // Start is called before the first frame update
    void Start()
    {
        GameObject cont = new GameObject("Container");
        cont.transform.parent = transform;
        container = cont.AddComponent<Container>();
        container.Initialize(sphereMaterial, Vector3.zero);
        float radius = 8.5f;
        Vector3 center = new Vector3(radius, radius, radius);

        for (int x = 0; x < 17; x++)
        {
            for (int y = 0; y < 17; y++)
            {
                for (int z = 0; z < 17; z++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    float distance = Vector3.Distance(pos, center);
                    if (distance <= radius)
                    {
                        container[pos] = new Voxel() { ID = 1 };
                    }
                }
            }
        }

        container.GenerateMesh();
        container.UploadMesh();
    }

    // Update is called once per frame
    void Update()
    {

    }

}

