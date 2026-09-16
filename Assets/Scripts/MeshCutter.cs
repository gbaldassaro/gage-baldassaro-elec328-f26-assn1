using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MeshCutter : MonoBehaviour
{

    public Material cutMaterial;
    public Color cutColor;

    public void CutMesh(Plane cutPlane, Vector3 torqueVector)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Material material = GetComponent<MeshRenderer>().material;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles; 
        Vector2[] uvs = mesh.uv;
        bool[] aboveCutPlane = new bool[vertices.Length];

        Mesh halfOne = new Mesh();
        halfOne.name = "Half One";
        List<Vector3> halfOneVertices = new List<Vector3>();
        List<int> halfOneTriangles = new List<int>(); 
        List<Vector2> halfOneUVs = new List<Vector2>();
        List<Vector3> halfOneEdgeVertices  = new List<Vector3>();

        Mesh halfTwo = new Mesh();
        halfTwo.name = "Half Two";
        List<Vector3> halfTwoVertices = new List<Vector3>();
        List<int> halfTwoTriangles = new List<int>(); 
        List<Vector2> halfTwoUVs = new List<Vector2>();
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

            Vector2 uvA = Vector2.zero;
            Vector2 uvB = Vector2.zero;
            Vector2 uvC = Vector2.zero;

            // case 1: all verts above cut
            if (aboveCutPlane[triangles[i]] && aboveCutPlane[triangles[i+1]] && aboveCutPlane[triangles[i+2]])
            {
                vertA = this.transform.TransformPoint(vertices[triangles[i]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i+1]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i+2]]);

                uvA = uvs[triangles[i]];
                uvB = uvs[triangles[i+1]];
                uvC = uvs[triangles[i+2]];

                int idx = halfOneVertices.Count;

                halfOneVertices.Add(vertA);
                halfOneVertices.Add(vertB);
                halfOneVertices.Add(vertC);

                halfOneUVs.Add(uvA);
                halfOneUVs.Add(uvB);
                halfOneUVs.Add(uvC);

                halfOneTriangles.Add(idx);
                halfOneTriangles.Add(idx+1);
                halfOneTriangles.Add(idx+2);

                continue;
            }

            // case 2: all verts below cut
            if (!aboveCutPlane[triangles[i]] && !aboveCutPlane[triangles[i+1]] && !aboveCutPlane[triangles[i+2]])
            {
                vertA = this.transform.TransformPoint(vertices[triangles[i]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i+1]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i+2]]);

                uvA = uvs[triangles[i]];
                uvB = uvs[triangles[i+1]];
                uvC = uvs[triangles[i+2]];

                int idx = halfTwoVertices.Count;

                halfTwoVertices.Add(vertA);
                halfTwoVertices.Add(vertB);
                halfTwoVertices.Add(vertC);

                halfTwoUVs.Add(uvA);
                halfTwoUVs.Add(uvB);
                halfTwoUVs.Add(uvC);

                halfTwoTriangles.Add(idx);
                halfTwoTriangles.Add(idx+1);
                halfTwoTriangles.Add(idx+2);

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

                uvA = uvs[triangles[i]];
                uvB = uvs[triangles[i+1]];
                uvC = uvs[triangles[i+2]];
            }
            else if (!aboveCutPlane[triangles[i]] && aboveCutPlane[triangles[i+1]] && aboveCutPlane[triangles[i+2]])
            {
                twoAbove = true;

                vertA = this.transform.TransformPoint(vertices[triangles[i+1]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i+2]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i]]);

                uvA = uvs[triangles[i+1]];
                uvB = uvs[triangles[i+2]];
                uvC = uvs[triangles[i]];
            }
            else if (aboveCutPlane[triangles[i]] && !aboveCutPlane[triangles[i+1]] && aboveCutPlane[triangles[i+2]])
            {
                twoAbove = true;

                vertA = this.transform.TransformPoint(vertices[triangles[i+2]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i+1]]);

                uvA = uvs[triangles[i+2]];
                uvB = uvs[triangles[i]];
                uvC = uvs[triangles[i+1]];
            }

            if (twoAbove)
            {
                Vector3 vertD1;
                Vector3 vertE1;

                Vector2 uvD1;
                Vector2 uvE1;

                float distFrom = cutPlane.GetDistanceToPoint(vertA);
                float distTo = cutPlane.GetDistanceToPoint(vertC);
                float t = Mathf.Clamp01(distFrom / (distFrom - distTo));
                vertD1 = Vector3.Lerp(vertA, vertC, t);
                uvD1 = Vector2.Lerp(uvA, uvC, t);

                distFrom = cutPlane.GetDistanceToPoint(vertB);
                distTo = cutPlane.GetDistanceToPoint(vertC);
                t = Mathf.Clamp01(distFrom / (distFrom - distTo));
                vertE1 = Vector3.Lerp(vertB, vertC, t);
                uvE1 = Vector2.Lerp(uvB, uvC, t);

                int idx = halfOneVertices.Count;

                halfOneVertices.Add(vertA);
                halfOneVertices.Add(vertB);
                halfOneVertices.Add(vertD1);
                halfOneVertices.Add(vertE1);

                halfOneUVs.Add(uvA);
                halfOneUVs.Add(uvB);
                halfOneUVs.Add(uvD1);
                halfOneUVs.Add(uvE1);

                halfOneEdgeVertices.Add(vertE1);
                halfOneEdgeVertices.Add(vertD1);

                halfOneTriangles.Add(idx);
                halfOneTriangles.Add(idx+1);
                halfOneTriangles.Add(idx+3);
                halfOneTriangles.Add(idx);
                halfOneTriangles.Add(idx+3);
                halfOneTriangles.Add(idx+2);

                Vector3 vertD2 = new Vector3(vertD1.x, vertD1.y, vertD1.z);
                Vector3 vertE2 = new Vector3(vertE1.x, vertE1.y, vertE1.z);

                Vector2 uvD2 = new Vector2(uvD1.x, uvD1.y);
                Vector2 uvE2 = new Vector2(uvE1.x, uvE1.y);

                idx = halfTwoVertices.Count;

                halfTwoVertices.Add(vertC);
                halfTwoVertices.Add(vertD2);
                halfTwoVertices.Add(vertE2);

                halfTwoUVs.Add(uvC);
                halfTwoUVs.Add(uvD2);
                halfTwoUVs.Add(uvE2);

                halfTwoEdgeVertices.Add(vertD2);
                halfTwoEdgeVertices.Add(vertE2);

                halfTwoTriangles.Add(idx);
                halfTwoTriangles.Add(idx+1);
                halfTwoTriangles.Add(idx+2);

                continue;
            }

            // case 4: one above
            if (aboveCutPlane[triangles[i]] && !aboveCutPlane[triangles[i+1]] && !aboveCutPlane[triangles[i+2]])
            {
                twoAbove = false;

                vertA = this.transform.TransformPoint(vertices[triangles[i]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i+1]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i+2]]);

                uvA = uvs[triangles[i]];
                uvB = uvs[triangles[i+1]];
                uvC = uvs[triangles[i+2]];
            }
            else if (!aboveCutPlane[triangles[i]] && aboveCutPlane[triangles[i+1]] && !aboveCutPlane[triangles[i+2]])
            {
                twoAbove = false;

                vertA = this.transform.TransformPoint(vertices[triangles[i+1]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i+2]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i]]);

                uvA = uvs[triangles[i+1]];
                uvB = uvs[triangles[i+2]];
                uvC = uvs[triangles[i]];
            }
            else if (!aboveCutPlane[triangles[i]] && !aboveCutPlane[triangles[i+1]] && aboveCutPlane[triangles[i+2]])
            {
                twoAbove = false;

                vertA = this.transform.TransformPoint(vertices[triangles[i+2]]);
                vertB = this.transform.TransformPoint(vertices[triangles[i]]);
                vertC = this.transform.TransformPoint(vertices[triangles[i+1]]);

                uvA = uvs[triangles[i+2]];
                uvB = uvs[triangles[i]];
                uvC = uvs[triangles[i+1]];
            }

            if (!twoAbove)
            {
                Vector3 vertD1;
                Vector3 vertE1;

                Vector2 uvD1;
                Vector2 uvE1;

                float distFrom = cutPlane.GetDistanceToPoint(vertA);
                float distTo = cutPlane.GetDistanceToPoint(vertB);
                float t = Mathf.Clamp01(distFrom / (distFrom - distTo));
                vertD1 = Vector3.Lerp(vertA, vertB, t);
                uvD1 = Vector2.Lerp(uvA, uvB, t);

                distFrom = cutPlane.GetDistanceToPoint(vertA);
                distTo = cutPlane.GetDistanceToPoint(vertC);
                t = Mathf.Clamp01(distFrom / (distFrom - distTo));
                vertE1 = Vector3.Lerp(vertA, vertC, t);
                uvE1 = Vector2.Lerp(uvA, uvC, t);

                int idx = halfTwoVertices.Count;

                halfTwoVertices.Add(vertB);
                halfTwoVertices.Add(vertC);
                halfTwoVertices.Add(vertD1);
                halfTwoVertices.Add(vertE1);

                halfTwoUVs.Add(uvB);
                halfTwoUVs.Add(uvC);
                halfTwoUVs.Add(uvD1);
                halfTwoUVs.Add(uvE1);

                halfTwoEdgeVertices.Add(vertE1);
                halfTwoEdgeVertices.Add(vertD1);

                halfTwoTriangles.Add(idx);
                halfTwoTriangles.Add(idx+1);
                halfTwoTriangles.Add(idx+3);
                halfTwoTriangles.Add(idx);
                halfTwoTriangles.Add(idx+3);
                halfTwoTriangles.Add(idx+2);

                Vector3 vertD2 = new Vector3(vertD1.x, vertD1.y, vertD1.z);
                Vector3 vertE2 = new Vector3(vertE1.x, vertE1.y, vertE1.z);

                Vector2 uvD2 = new Vector2(uvD1.x, uvD1.y);
                Vector2 uvE2 = new Vector2(uvE1.x, uvE1.y);

                idx = halfOneVertices.Count;

                halfOneVertices.Add(vertA);
                halfOneVertices.Add(vertD2);
                halfOneVertices.Add(vertE2);

                halfOneUVs.Add(uvA);
                halfOneUVs.Add(uvD2);
                halfOneUVs.Add(uvE2);

                halfOneEdgeVertices.Add(vertD2);
                halfOneEdgeVertices.Add(vertE2);

                halfOneTriangles.Add(idx);
                halfOneTriangles.Add(idx+1);
                halfOneTriangles.Add(idx+2);

                continue;
            }
        }

        Vector3 sumOne = Vector3.zero;
        foreach (Vector3 vertex in halfOneEdgeVertices)
        {
            sumOne += vertex;
        }
        Vector3 centerOne = sumOne / halfOneEdgeVertices.Count;
        
        Mesh halfOneCut = new Mesh();
        halfOneCut.name = "Half One Cut";
        List<Vector3> halfOneCutVertices = new List<Vector3>(halfOneEdgeVertices);
        halfOneCutVertices.Add(centerOne);
        List<Vector2> halfOneCutUVs = Enumerable.Repeat(Vector2.zero, halfOneCutVertices.Count).ToList();
        List<int> halfOneCutTriangles = new List<int>(); 

        for (int i = 0; i < halfOneEdgeVertices.Count; i+=2)
        {
            halfOneCutTriangles.Add(halfOneCutVertices.IndexOf(halfOneEdgeVertices[i%halfOneEdgeVertices.Count]));
            halfOneCutTriangles.Add(halfOneCutVertices.IndexOf(centerOne));
            halfOneCutTriangles.Add(halfOneCutVertices.IndexOf(halfOneEdgeVertices[(i+1)%halfOneEdgeVertices.Count]));
        }

        Vector3 sumTwo = Vector3.zero;
        foreach (Vector3 vertex in halfTwoEdgeVertices)
        {
            sumTwo += vertex;
        }
        Vector3 centerTwo = sumTwo / halfTwoEdgeVertices.Count;
        
        Mesh halfTwoCut = new Mesh();
        halfTwoCut.name = "Half Two Cut";
        List<Vector3> halfTwoCutVertices = new List<Vector3>(halfTwoEdgeVertices);
        halfTwoCutVertices.Add(centerTwo);
        List<Vector2> halfTwoCutUVs = Enumerable.Repeat(Vector2.zero, halfTwoCutVertices.Count).ToList();
        List<int> halfTwoCutTriangles = new List<int>(); 

        for (int i = 0; i < halfTwoEdgeVertices.Count; i+=2)
        {
            halfTwoCutTriangles.Add(halfTwoCutVertices.IndexOf(halfTwoEdgeVertices[i%halfTwoEdgeVertices.Count]));
            halfTwoCutTriangles.Add(halfTwoCutVertices.IndexOf(centerTwo));
            halfTwoCutTriangles.Add(halfTwoCutVertices.IndexOf(halfTwoEdgeVertices[(i+1)%halfTwoEdgeVertices.Count]));
        }

        for (int i = 0; i < halfOneVertices.Count; i++)
        {
            halfOneVertices[i] = this.transform.InverseTransformPoint(halfOneVertices[i]);
        }
        for (int i = 0; i < halfTwoVertices.Count; i++)
        {
            halfTwoVertices[i] = this.transform.InverseTransformPoint(halfTwoVertices[i]);
        }

        for (int i = 0; i < halfOneCutVertices.Count; i++)
        {
            halfOneCutVertices[i] = this.transform.InverseTransformPoint(halfOneCutVertices[i]);
        }
        for (int i = 0; i < halfTwoCutVertices.Count; i++)
        {
            halfTwoCutVertices[i] = this.transform.InverseTransformPoint(halfTwoCutVertices[i]);
        }

        halfOne.vertices = halfOneVertices.ToArray();
        halfOne.uv = halfOneUVs.ToArray();
        halfOne.triangles = halfOneTriangles.ToArray();
        halfOne.RecalculateNormals();
        halfOne.RecalculateBounds();

        halfOneCut.vertices = halfOneCutVertices.ToArray();
        halfOneCut.uv = halfOneCutUVs.ToArray();
        halfOneCut.triangles = halfOneCutTriangles.ToArray();
        halfOneCut.RecalculateNormals();
        halfOneCut.RecalculateBounds();

        halfTwo.vertices = halfTwoVertices.ToArray();
        halfTwo.uv = halfTwoUVs.ToArray();
        halfTwo.triangles = halfTwoTriangles.ToArray();
        halfTwo.RecalculateNormals();
        halfTwo.RecalculateBounds();

        halfTwoCut.vertices = halfTwoCutVertices.ToArray();
        halfTwoCut.uv = halfTwoCutUVs.ToArray();
        halfTwoCut.triangles = halfTwoCutTriangles.ToArray();
        halfTwoCut.RecalculateNormals();
        halfTwoCut.RecalculateBounds();

        GameObject halfOneObj = new GameObject(this.gameObject.name + "Half One");
        halfOneObj.transform.position = this.transform.position;
        halfOneObj.transform.rotation = this.transform.rotation;
        halfOneObj.transform.localScale = this.transform.localScale;
        halfOneObj.AddComponent<MeshFilter>();
        halfOneObj.AddComponent<MeshRenderer>();
        halfOneObj.AddComponent<MeshCollider>();
        halfOneObj.AddComponent<Rigidbody>();
        // halfOneObj.AddComponent<MeshCutter>();

        halfOneObj.GetComponent<MeshFilter>().mesh = halfOne;
        halfOneObj.GetComponent<MeshRenderer>().material = material;
        halfOneObj.GetComponent<MeshCollider>().sharedMesh = halfOne;
        halfOneObj.GetComponent<MeshCollider>().convex = true;
        // halfOneObj.GetComponent<MeshCutter>().cutMaterial = cutMaterial;
        // halfOneObj.GetComponent<MeshCutter>().cutColor = cutColor;
        // halfOneObj.GetComponent<MeshCutter>().plane = plane;

        GameObject halfOneCutObj = new GameObject();
        halfOneCutObj.transform.SetParent(halfOneObj.transform);
        halfOneCutObj.transform.position = this.transform.position;
        halfOneCutObj.transform.rotation = this.transform.rotation;
        halfOneCutObj.transform.localScale = Vector3.one;
        halfOneCutObj.AddComponent<MeshFilter>();
        halfOneCutObj.AddComponent<MeshRenderer>();
        // halfOneCutObj.AddComponent<MeshCutter>();

        halfOneCutObj.GetComponent<MeshFilter>().mesh = halfOneCut;
        halfOneCutObj.GetComponent<MeshRenderer>().material = cutMaterial;
        halfOneCutObj.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", cutColor);
        // halfOneCutObj.GetComponent<MeshCutter>().cutMaterial = cutMaterial;
        // halfOneCutObj.GetComponent<MeshCutter>().cutColor = cutColor;
        // halfOneCutObj.GetComponent<MeshCutter>().plane = plane;

        GameObject halfTwoObj = new GameObject(this.gameObject.name + " Half Two");
        halfTwoObj.transform.position = this.transform.position;
        halfTwoObj.transform.rotation = this.transform.rotation;
        halfTwoObj.transform.localScale = this.transform.localScale;
        halfTwoObj.AddComponent<MeshFilter>();
        halfTwoObj.AddComponent<MeshRenderer>();
        halfTwoObj.AddComponent<MeshCollider>();
        halfTwoObj.AddComponent<Rigidbody>();
        // halfTwoObj.AddComponent<MeshCutter>();

        halfTwoObj.GetComponent<MeshFilter>().mesh = halfTwo;
        halfTwoObj.GetComponent<MeshRenderer>().material = material;
        halfTwoObj.GetComponent<MeshCollider>().sharedMesh = halfTwo;
        halfTwoObj.GetComponent<MeshCollider>().convex = true;
        // halfTwoObj.GetComponent<MeshCutter>().cutMaterial = cutMaterial;
        // halfTwoObj.GetComponent<MeshCutter>().cutColor = cutColor;
        // halfTwoObj.GetComponent<MeshCutter>().plane = plane;

        GameObject halfTwoCutObj = new GameObject();
        halfTwoCutObj.transform.SetParent(halfTwoObj.transform);
        halfTwoCutObj.transform.position = this.transform.position;
        halfTwoCutObj.transform.rotation = this.transform.rotation;
        halfTwoCutObj.transform.localScale = Vector3.one;
        halfTwoCutObj.AddComponent<MeshFilter>();
        halfTwoCutObj.AddComponent<MeshRenderer>();
        // halfTwoCutObj.AddComponent<MeshCutter>();

        halfTwoCutObj.GetComponent<MeshFilter>().mesh = halfTwoCut;
        halfTwoCutObj.GetComponent<MeshRenderer>().material = cutMaterial;
        halfTwoCutObj.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", cutColor);
        // halfTwoCutObj.GetComponent<MeshCutter>().cutMaterial = cutMaterial;
        // halfTwoCutObj.GetComponent<MeshCutter>().cutColor = cutColor;
        // halfTwoCutObj.GetComponent<MeshCutter>().plane = plane;

        Destroy(gameObject);

        halfOneObj.GetComponent<Rigidbody>().AddForce(cutPlane.normal * 10f);
        halfTwoObj.GetComponent<Rigidbody>().AddForce(-cutPlane.normal * 10f);
        halfOneObj.GetComponent<Rigidbody>().AddTorque(torqueVector * 10f);
        halfTwoObj.GetComponent<Rigidbody>().AddTorque(-torqueVector * 10f);

    } 
}
