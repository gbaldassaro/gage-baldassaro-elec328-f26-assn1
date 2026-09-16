using UnityEngine;

public class Sword : MonoBehaviour
{
    private Transform controller;
    public bool right;
    private Transform cutPlane;

    void Awake()
    {
        // controller = right ? GameObject.Find("Right Controller").transform : GameObject.Find("Left Controller").transform;
        // this.transform.SetParent(controller);
        // this.transform.localRotation = Quaternion.Euler(35.0f, 0.0f, 0.0f);
        cutPlane = GameObject.FindGameObjectWithTag("Cut Plane").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<MeshCutter>(out MeshCutter meshCutter))
        {
            meshCutter.CutMesh(new Plane(cutPlane.up, cutPlane.position), cutPlane.forward);
        }
    }

}
