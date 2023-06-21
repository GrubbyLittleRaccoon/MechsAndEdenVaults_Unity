using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Further explanation: https://www.youtube.com/watch?v=64NblGkAabk&t=72s
// This is an alternative mesh generator to our perlin terrain.
// I think the concept is a bit more oriented to creating perlin textures before applying
// Don't think I ever got this working unfortunately, seems that this and the working perlin file were created at the same time

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    // Start is called before the first frame update (Default class function)
    void Start(){
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape(){
        // Create an x * z plane of vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)]; // Store all vertices
        for(int i=0, z=0; z<=zSize; z++) {
            for(int x=0; x <= xSize; x++) {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f; // TODO Find perlin noise video and mess with values
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        // Create triangles in squares (triangle pairs)
        // TODO: Experiment with variable mesh size

        triangles = new int[xSize * zSize * 6]; // Triangle index array. Note this links to vertices
        int tris = 0; // Triangle point counter
        int vert = 0; // x-es passed Counter

        for (int z = 0; z < zSize; z++) {

            for (int x = 0; x < xSize; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++; // Skip triangle linking row end to next row
        }
    }

    // Update is called once per frame (Default class function)
    void Update(){
        UpdateMesh();
    }

    // Push internal values to mesh member
    void UpdateMesh(){
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Have to update normals for triangles after setting locations
    }
}
