using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
    [SerializeField] private bool right;
    [SerializeField] private Transform cutPlane;

    private Transform controller;
    private Quaternion localRotation = Quaternion.Euler(35.0f, 0.0f, 0.0f);

    IEnumerator Start()
    {
        string tag = right ? "Right Controller" : "Left Controller";
        while(controller == null)
        {
            GameObject foundController = GameObject.FindWithTag(tag);
            if (foundController != null)
            {
                controller = foundController.transform;
            }
            yield return null;
        }
    }

    private void LateUpdate()
    {
        if (controller != null)
        {
            transform.position = controller.position;
            transform.rotation = controller.rotation * localRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MeshCutter>(out MeshCutter meshCutter))
        {
            meshCutter.CutMesh(new Plane(cutPlane.up, cutPlane.position), cutPlane.forward);

            if (other.TryGetComponent<OptionSlice>(out OptionSlice optionSlice))
            {
                StartCoroutine(optionSlice.Activate());
            }
        }
    }

}
