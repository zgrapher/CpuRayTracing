using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CubeArray : MonoBehaviour
{
    public GameObject cube;
    public int width = 10;
    public int length = 10;
    public float space = 1.2f;
    public float heightMax = 8.0f;
    public float heightMin = 6.0f;
    public float sizeMax = 1.0f;
    public float sizeMin = 0.8f;

    private List<GameObject> obj_list = new List<GameObject>();

    public void Build()
    {
        Clear();

        var posX = 0.0f;
        var posZ = 0.0f;
        for (var z = 0; z < length; ++z)
        {
            for (var x = 0; x < width; x++)
            {
                GameObject newObj = Instantiate(cube, transform.position, quaternion.identity);
                newObj.transform.parent = transform;
                var size = Random.Range(sizeMin, sizeMax);
                var height = Random.Range(heightMin, heightMax);
                var pos = new Vector3(posX, height * 0.5f, posZ);
                
                newObj.transform.localPosition = pos;
                newObj.transform.localScale = new Vector3(size, height, size);
                obj_list.Add(newObj);
                
                posX += space;
            }

            posX = 0.0f;
            posZ += space;
        }
    }

    public void Clear()
    {
        foreach (GameObject obj in obj_list)
        {
            DestroyImmediate(obj);
        }
        obj_list.Clear();
    }
}
