using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveMesh : MonoBehaviour
{

    public int subdivisions = default; // 원하는 해상도 조절

    public MeshFilter origin;
    public Mesh copy;
    public float scaleMultiple;
    public string savePath = "Assets/";

    private void Awake()
    {

        copy = CreateSphere(subdivisions);

    }

    private void Start()
    {
        RegenerateMesh(scaleMultiple);
        SaveMeshed();
    }

    public void RegenerateMesh(float scaleMultiple)
    {
        Vector3[] vertices = copy.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= scaleMultiple;
        }
        copy.vertices = vertices;
        copy.RecalculateBounds();


    }

    Mesh CreateSphere(int subdivisions)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Big3Sphere";

        Vector3[] vertices = new Vector3[(subdivisions + 1) * (subdivisions + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[subdivisions * subdivisions * 6];

        for (int i = 0; i <= subdivisions; i++)
        {
            for (int j = 0; j <= subdivisions; j++)
            {
                float phi = Mathf.PI * i / subdivisions;
                float theta = 2 * Mathf.PI * j / subdivisions;
                float x = Mathf.Sin(phi) * Mathf.Cos(theta);
                float y = Mathf.Cos(phi);
                float z = Mathf.Sin(phi) * Mathf.Sin(theta);

                int index = i * (subdivisions + 1) + j;
                vertices[index] = new Vector3(x, y, z);
                uv[index] = new Vector2((float)j / subdivisions, (float)i / subdivisions);
            }
        }

        int triangleIndex = 0;
        for (int i = 0; i < subdivisions; i++)
        {
            for (int j = 0; j < subdivisions; j++)
            {
                int vertexIndex = i * (subdivisions + 1) + j;
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + 1;
                triangles[triangleIndex + 2] = vertexIndex + subdivisions + 1;
                triangles[triangleIndex + 3] = vertexIndex + subdivisions + 1;
                triangles[triangleIndex + 4] = vertexIndex + 1;
                triangles[triangleIndex + 5] = vertexIndex + subdivisions + 2;

                triangleIndex += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    public void SaveMeshed()
    {
        if (copy != null)
        {
            string meshName = copy.name + ".asset";
            string fullPath = Path.Combine(savePath, meshName);

            // Mesh를 Asset 파일로 저장
            AssetDatabase.CreateAsset(copy, fullPath);
            AssetDatabase.SaveAssets();
            Debug.Log("Mesh saved to: " + fullPath);
        }
        else
        {
            Debug.LogError("Mesh is null. Cannot save.");
        }
    }

}
