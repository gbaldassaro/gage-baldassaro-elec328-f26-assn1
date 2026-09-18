using UnityEngine;

public class SpawnController : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    void Start()
    {
        originalPosition = this.transform.position;
        originalRotation = this.transform.rotation;
        originalScale = this.transform.localScale;
    }

    public void Hide()
    {
        this.transform.position = new Vector3(0, -100, 0);
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
    }

    public void Reset()
    {
        foreach (Transform child in this.transform) 
        {
            Destroy(child.gameObject);
        }
        
        this.transform.SetPositionAndRotation(originalPosition, originalRotation);
        this.transform.localScale = originalScale;
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false;
        }
    }
}
