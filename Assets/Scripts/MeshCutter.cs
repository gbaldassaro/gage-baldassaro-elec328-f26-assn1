using UnityEngine;
using System.Collections.Generic;

public class MeshCutter : MonoBehaviour
{

    public Transform plane;
    public bool cut = false;

    private Material material;

    void Start()
    {
        
    }

    void Update()
    {
        if (cut)
        {
            CutMesh(new Plane(plane.transform.up, plane.transform.position));
            cut = false;
        }
    }

    private void CutMesh(Plane cutPlane)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Material material = GetComponent<MeshRenderer>().material;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles; 
        bool[] aboveCutPlane = new bool[vertices.Length];

        Mesh halfOne = new Mesh();
        halfOne.name = "Half One";
        List<Vector3> halfOneVertices = new List<Vector3>();
        List<int> halfOneTriangles = new List<int>(); 
        List<Vector3> halfOneEdgeVertices  = new List<Vector3>();

        Mesh halfTwo = new Mesh();
        halfTwo.name = "Half Two";
        List<Vector3> halfTwoVertices = new List<Vector3>();
        List<int> halfTwoTriangles = new List<int>(); 
        List<Vector3> halfTwoEdgeVertices  = new List<Vector3>();

        
        for (int i = 0; i < vertices.Length; i++)
        {
            aboveCutPlane[i] = cutPlane.GetSide(this.transform.TransformPoint(vertices[i]));
        }

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 vertA = Vector3.zero;
            Vector3 vertB = Vector3.zero;
            Vector3 vertC = Vector3.zero;

            // case 1: all verts above cut
            if (aboveCutPlane[triangles[i]] && aboveCutPlane[triangles[i+1]] && aboveCutPlane[triangles[i+2]])
            {
                vertA = this.transform.TransformPoint(vertices[triangles[i]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i+1]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i+2]]);

                halfOneVertices.Add(vertA);
                halfOneVertices.Add(vertB);
                halfOneVertices.Add(vertC);

                halfOneTriangles.Add(halfOneVertices.IndexOf(vertA));
                halfOneTriangles.Add(halfOneVertices.IndexOf(vertB));
                halfOneTriangles.Add(halfOneVertices.IndexOf(vertC));

                continue;
            }

            // case 2: all verts below cut
            if (!aboveCutPlane[triangles[i]] && !aboveCutPlane[triangles[i+1]] && !aboveCutPlane[triangles[i+2]])
            {
                vertA = this.transform.TransformPoint(vertices[triangles[i]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i+1]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i+2]]);

                halfTwoVertices.Add(vertA);
                halfTwoVertices.Add(vertB);
                halfTwoVertices.Add(vertC);

                halfTwoTriangles.Add(halfTwoVertices.IndexOf(vertA));
                halfTwoTriangles.Add(halfTwoVertices.IndexOf(vertB));
                halfTwoTriangles.Add(halfTwoVertices.IndexOf(vertC));

                continue;
            }

            bool twoAbove = false;
            // case 3: two above
            if (aboveCutPlane[triangles[i]] && aboveCutPlane[triangles[i+1]] && !aboveCutPlane[triangles[i+2]])
            {
                twoAbove = true;

                vertA = this.transform.TransformPoint(vertices[triangles[i]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i+1]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i+2]]);
            }
            else if (!aboveCutPlane[triangles[i]] && aboveCutPlane[triangles[i+1]] && aboveCutPlane[triangles[i+2]])
            {
                twoAbove = true;

                vertA = this.transform.TransformPoint(vertices[triangles[i+1]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i+2]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i]]);
            }
            else if (aboveCutPlane[triangles[i]] && !aboveCutPlane[triangles[i+1]] && aboveCutPlane[triangles[i+2]])
            {
                twoAbove = true;

                vertA = this.transform.TransformPoint(vertices[triangles[i+2]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i+1]]);
            }

            if (twoAbove)
            {
                Vector3 vertD1 = new Vector3();
                Vector3 vertE1 = new Vector3();

                float distFrom = cutPlane.GetDistanceToPoint(vertA);
                float distTo = cutPlane.GetDistanceToPoint(vertC);
                float t = Mathf.Clamp01(distFrom / (distFrom - distTo));
                vertD1 = Vector3.Lerp(vertA, vertC, t);

                distFrom = cutPlane.GetDistanceToPoint(vertB);
                distTo = cutPlane.GetDistanceToPoint(vertC);
                t = Mathf.Clamp01(distFrom / (distFrom - distTo));
                vertE1 = Vector3.Lerp(vertB, vertC, t);

                halfOneVertices.Add(vertA);
                halfOneVertices.Add(vertB);
                halfOneVertices.Add(vertD1);
                halfOneVertices.Add(vertE1);

                halfOneEdgeVertices.Add(vertE1);
                halfOneEdgeVertices.Add(vertD1);

                halfOneTriangles.Add(halfOneVertices.IndexOf(vertA));
                halfOneTriangles.Add(halfOneVertices.IndexOf(vertB));
                halfOneTriangles.Add(halfOneVertices.IndexOf(vertE1));
                halfOneTriangles.Add(halfOneVertices.IndexOf(vertA));
                halfOneTriangles.Add(halfOneVertices.IndexOf(vertE1));
                halfOneTriangles.Add(halfOneVertices.IndexOf(vertD1));

                Vector3 vertD2 = new Vector3(vertD1.x, vertD1.y, vertD1.z);
                Vector3 vertE2 = new Vector3(vertE1.x, vertE1.y, vertE1.z);

                halfTwoVertices.Add(vertC);
                halfTwoVertices.Add(vertD2);
                halfTwoVertices.Add(vertE2);

                halfTwoEdgeVertices.Add(vertD2);
                halfTwoEdgeVertices.Add(vertE2);

                halfTwoTriangles.Add(halfTwoVertices.IndexOf(vertC));
                halfTwoTriangles.Add(halfTwoVertices.IndexOf(vertD2));
                halfTwoTriangles.Add(halfTwoVertices.IndexOf(vertE2));

                continue;
            }

            // case 4: one above
            if (aboveCutPlane[triangles[i]] && !aboveCutPlane[triangles[i+1]] && !aboveCutPlane[triangles[i+2]])
            {
                twoAbove = false;

                vertA = this.transform.TransformPoint(vertices[triangles[i]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i+1]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i+2]]);
            }
            else if (!aboveCutPlane[triangles[i]] && aboveCutPlane[triangles[i+1]] && !aboveCutPlane[triangles[i+2]])
            {
                twoAbove = false;

                vertA = this.transform.TransformPoint(vertices[triangles[i+1]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i+2]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i]]);
            }
            else if (!aboveCutPlane[triangles[i]] && !aboveCutPlane[triangles[i+1]] && aboveCutPlane[triangles[i+2]])
            {
                twoAbove = false;

                vertA = this.transform.TransformPoint(vertices[triangles[i+2]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i+1]]);
            }

            if (!twoAbove)
            {
                Vector3 vertD1 = new Vector3();
                Vector3 vertE1 = new Vector3();

                float distFrom = cutPlane.GetDistanceToPoint(vertA);
                float distTo = cutPlane.GetDistanceToPoint(vertB);
                float t = Mathf.Clamp01(distFrom / (distFrom - distTo));
                vertD1 = Vector3.Lerp(vertA, vertB, t);

                distFrom = cutPlane.GetDistanceToPoint(vertA);
                distTo = cutPlane.GetDistanceToPoint(vertC);
                t = Mathf.Clamp01(distFrom / (distFrom - distTo));
                vertE1 = Vector3.Lerp(vertA, vertC, t);

                halfTwoVertices.Add(vertB);
                halfTwoVertices.Add(vertC);
                halfTwoVertices.Add(vertD1);
                halfTwoVertices.Add(vertE1);

                halfTwoEdgeVertices.Add(vertE1);
                halfTwoEdgeVertices.Add(vertD1);

                halfTwoTriangles.Add(halfTwoVertices.IndexOf(vertB));
                halfTwoTriangles.Add(halfTwoVertices.IndexOf(vertC));
                halfTwoTriangles.Add(halfTwoVertices.IndexOf(vertE1));
                halfTwoTriangles.Add(halfTwoVertices.IndexOf(vertB));
                halfTwoTriangles.Add(halfTwoVertices.IndexOf(vertE1));
                halfTwoTriangles.Add(halfTwoVertices.IndexOf(vertD1));

                Vector3 vertD2 = new Vector3(vertD1.x, vertD1.y, vertD1.z);
                Vector3 vertE2 = new Vector3(vertE1.x, vertE1.y, vertE1.z);

                halfOneVertices.Add(vertA);
                halfOneVertices.Add(vertD2);
                halfOneVertices.Add(vertE2);

                halfOneEdgeVertices.Add(vertD2);
                halfOneEdgeVertices.Add(vertE2);

                halfOneTriangles.Add(halfOneVertices.IndexOf(vertA));
                halfOneTriangles.Add(halfOneVertices.IndexOf(vertD2));
                halfOneTriangles.Add(halfOneVertices.IndexOf(vertE2));

                continue;
            }
        }

        Vector3 sumOne = Vector3.zero;
        foreach (Vector3 vertex in halfOneEdgeVertices)
        {
            sumOne += vertex;
        }
        Vector3 centerOne = sumOne / halfOneEdgeVertices.Count;
        halfOneVertices.Add(centerOne);

        for (int i = 0; i < halfOneEdgeVertices.Count; i+=2)
        {
            halfOneTriangles.Add(halfOneVertices.IndexOf(halfOneEdgeVertices[i%halfOneEdgeVertices.Count]));
            halfOneTriangles.Add(halfOneVertices.IndexOf(centerOne));
            halfOneTriangles.Add(halfOneVertices.IndexOf(halfOneEdgeVertices[(i+1)%halfOneEdgeVertices.Count]));
        }

        Vector3 sumTwo = Vector3.zero;
        foreach (Vector3 vertex in halfTwoEdgeVertices)
        {
            sumTwo += vertex;
        }
        Vector3 centerTwo = sumTwo / halfTwoEdgeVertices.Count;
        halfTwoVertices.Add(centerTwo);

        for (int i = 0; i < halfTwoEdgeVertices.Count; i+=2)
        {
            halfTwoTriangles.Add(halfTwoVertices.IndexOf(halfTwoEdgeVertices[i%halfTwoEdgeVertices.Count]));
            halfTwoTriangles.Add(halfTwoVertices.IndexOf(centerTwo));
            halfTwoTriangles.Add(halfTwoVertices.IndexOf(halfTwoEdgeVertices[(i+1)%halfTwoEdgeVertices.Count]));
        }

        // for (int i = 0; i < halfOneVertices.Count; i++)
        // {
        //     halfOneVertices[i] = this.transform.InverseTransformPoint(halfOneVertices[i]);
        // }
        // for (int i = 0; i < halfTwoVertices.Count; i++)
        // {
        //     halfTwoVertices[i] = this.transform.InverseTransformPoint(halfTwoVertices[i]);
        // }

        halfOne.vertices = halfOneVertices.ToArray();
        halfOne.triangles = halfOneTriangles.ToArray();
        halfOne.RecalculateNormals();
        halfOne.RecalculateBounds();

        halfTwo.vertices = halfTwoVertices.ToArray();
        halfTwo.triangles = halfTwoTriangles.ToArray();
        halfTwo.RecalculateNormals();
        halfTwo.RecalculateBounds();

        GameObject halfOneObj = new GameObject();
        halfOneObj.AddComponent<MeshFilter>();
        halfOneObj.AddComponent<MeshRenderer>();
        halfOneObj.AddComponent<MeshCollider>();
        halfOneObj.AddComponent<Rigidbody>();
        halfOneObj.AddComponent<MeshCutter>();

        halfOneObj.GetComponent<MeshFilter>().mesh = halfOne;
        halfOneObj.GetComponent<MeshRenderer>().material = material;
        halfOneObj.GetComponent<MeshCollider>().sharedMesh = halfOne;
        halfOneObj.GetComponent<MeshCollider>().convex = true;
        // halfOneObj.GetComponent<Rigidbody>().useGravity = false;

        GameObject halfTwoObj = new GameObject();
        halfTwoObj.AddComponent<MeshFilter>();
        halfTwoObj.AddComponent<MeshRenderer>();
        halfTwoObj.AddComponent<MeshCollider>();
        halfTwoObj.AddComponent<Rigidbody>();
        halfTwoObj.AddComponent<MeshCutter>();

        halfTwoObj.GetComponent<MeshFilter>().mesh = halfTwo;
        halfTwoObj.GetComponent<MeshRenderer>().material = material;
        halfTwoObj.GetComponent<MeshCollider>().sharedMesh = halfTwo;
        halfTwoObj.GetComponent<MeshCollider>().convex = true;
        // halfTwoObj.GetComponent<Rigidbody>().useGravity = false;

        Destroy(gameObject);

    } 
}
