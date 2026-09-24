using UnityEngine;

public class SwordTrail : MonoBehaviour
{
    [SerializeField] private Transform tip;
    [SerializeField] private Transform swordBase;

    private Vector3 tipPosFrameAgo;
    private Vector3 swordBasePosFrameAgo;
    private Vector3 tipPosTwoFramesAgo;
    private Vector3 swordBasePosTwoFramesAgo;

    [SerializeField] private Material material;

    private Vector3[] verts;
    private int[] tris;

    private GameObject trail;
    private Mesh mesh;

    void Start()
    {
        tipPosFrameAgo = tip.position;
        swordBasePosFrameAgo = swordBase.position;

        trail = new GameObject("Sword Trail");
        mesh = new Mesh();

        trail.AddComponent<MeshFilter>().mesh = mesh;
        trail.AddComponent<MeshRenderer>().material = material;

        verts = new Vector3[6];

        tris = new int[24]
        {
            0, 1, 4,  0, 4, 5, // Front face past
            0, 4, 1,  0, 5, 4,  // Back face past
            1, 2, 3,  1, 3, 4, // Front face recent
            1, 3, 2,  1, 4, 3  // Back face recent
        };
        
        mesh.vertices = verts;
        mesh.triangles = tris;
    }

    void LateUpdate()
    {
        verts[0] = swordBasePosTwoFramesAgo;
        verts[1] = swordBasePosFrameAgo;
        verts[2] = swordBase.position;
        verts[3] = tip.position;
        verts[4] = tipPosFrameAgo;
        verts[5] = tipPosTwoFramesAgo;

        mesh.vertices = verts;
        mesh.RecalculateBounds();

        tipPosTwoFramesAgo = tipPosFrameAgo;
        tipPosFrameAgo = tip.position;
        swordBasePosTwoFramesAgo = swordBasePosFrameAgo;
        swordBasePosFrameAgo = swordBase.position;
    }
}