using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineMeshBuilder : MonoBehaviour
{
    List<Vector3> rightVector;
    List<Vector3> leftVector;

    [SerializeField]
    int resolution = 100;
    [SerializeField]
    float height = 4f;

    [SerializeField]
    SamplingSpline sampling;

    List<GameObject> upTerrains;
    List<GameObject> leftTerrains;
    List<GameObject> rightTerrains;

    private void Awake()
    {
        upTerrains = new List<GameObject>();
        leftTerrains = new List<GameObject>();
        rightTerrains = new List<GameObject>();
    }

    void Start()
    {
        for(int i = 0; i < sampling.GetSplineCount(); i++)
        {
            Generate(i);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Generate(0);
        }
    }

    void Generate(int index)
    {
        GameObject parent = new GameObject();
        parent.transform.parent = transform;
        GetVerts(index);
        Build(parent);

    }

    void GetVerts(int index)
    {
        rightVector = new List<Vector3>();
        leftVector = new List<Vector3>();

        float step = 1 / (float)resolution;

        for(int i = 0; i < resolution; i++)
        {
            float t = step * i;
            sampling.SampleSplineWidth(t, out Vector3 vv1, out Vector3 vv2);
            rightVector.Add(vv1);
            leftVector.Add(vv2);
        }
    }

    GameObject ConcreateGameObject(Material material)
    {
        GameObject result = new GameObject();
        result.AddComponent<MeshFilter>();
        result.AddComponent<MeshRenderer>().material = material;
        result.AddComponent<MeshCollider>();

        return result;
    }

    void Build(GameObject parent)
    {
        GameObject up = ConcreateGameObject(null);
        up.transform.parent = parent.transform;
        BuildUp(up);

        GameObject right = ConcreateGameObject(null);
        right.transform.parent = parent.transform;
        BuildSide(right, rightVector);

        GameObject left = ConcreateGameObject(null);
        left.transform.parent = parent.transform;
        leftVector.Reverse();
        BuildSide(left, leftVector);
    }

    void BuildUp(GameObject gameObject)
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        int offset = 0;

        int length = leftVector.Count;

        for(int i = 1; i <=length; i++)
        {
            Vector3 p1 = rightVector[i - 1];
            Vector3 p2 = leftVector[i - 1];
            Vector3 p3;
            Vector3 p4;

            if (i == length)
            {
                if (!sampling.IsClosed())
                {
                    break;
                }
                p3 = rightVector[0];
                p4 = leftVector[0];
            }
            else
            {
                Debug.LogError(i);
                p3 = rightVector[i];
                p4 = leftVector[i];
            }

            offset = 4 * (i - 1);

            int t1 = offset + 0;
            int t2 = offset + 2;
            int t3 = offset + 3;


            int t4 = offset + 3;
            int t5 = offset + 1;
            int t6 = offset + 0;

            verts.AddRange(new List<Vector3> { p1, p2, p3, p4 });
            tris.AddRange(new List<int> { t1,t2,t3,t4,t5,t6 });

        }


        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

    }

        
    void BuildSide(GameObject gameObject, List<Vector3> v)
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        int offset = 0;

        int length = v.Count;

        for (int i = 1; i <= length; i++)
        {
            Vector3 p1 = v[i-1]-Vector3.up* height;
            Vector3 p2 = v[i-1];
            Vector3 p3;
            Vector3 p4;

            if (i == length)
            {
                if (!sampling.IsClosed())
                {
                    break;
                }
                p3 = v[0] - Vector3.up * height;
                p4 = v[0];
            }
            else
            {
                p3 = v[i] - Vector3.up * height;
                p4 = v[i];
            }

            offset = 4 * (i - 1);

            int t1 = offset + 0;
            int t2 = offset + 2;
            int t3 = offset + 3;


            int t4 = offset + 3;
            int t5 = offset + 1;
            int t6 = offset + 0;

            verts.AddRange(new List<Vector3> { p1, p2, p3, p4 });
            tris.AddRange(new List<int> { t1, t2, t3, t4, t5, t6 });

        }
        

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

    }
}
