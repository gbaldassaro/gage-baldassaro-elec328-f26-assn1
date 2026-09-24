using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(SpawnController))]
public class Ripple : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] verts;
    private Vector3[] norms;
    private Vector3[] newVerts;
    private float startTime;

    void Start()
    {
        startTime = Time.time;

        mesh = GetComponent<MeshFilter>().mesh;
        verts = mesh.vertices;
        newVerts = mesh.vertices;
        norms = mesh.normals;
        StartCoroutine(HideTimer());
    }

    void Update()
    {
        float elapsedTime = startTime - Time.time;
        for (int i = 0; i < verts.Length; i++)
        {
            newVerts[i] = transform.TransformPoint(verts[i]) + Vector3.up * (elapsedTime * Mathf.Sin(0.5f * elapsedTime * elapsedTime + 10.0f * verts[i].x) / 200.0f);
            newVerts[i] = transform.InverseTransformPoint(newVerts[i]);
        }
        mesh.vertices = newVerts;
        mesh.RecalculateBounds();
    }

    private IEnumerator HideTimer()
    {
        yield return new WaitForSeconds(10);
        while (transform.localScale.x > 0.1)
        {
            Vector3 scale = transform.localScale;
            scale -= Vector3.one * 0.1f;
            transform.localScale = scale;
            yield return null;
        }
        GetComponent<SpawnController>().Hide();
    }
}
