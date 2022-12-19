using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    public int xSize = 1;
    // Start is called before the first frame update
    public void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
       /*TODO:
        * To make it granular based: Each Tile should have NxN vertices, N = Bitrate
        * FUTURE IDEA: Prefab for each tile / wall such that there can be variation in "brick location"
        * For now: each tile uses 4 vertices       
        * 
        * 
        * 
        */       
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
