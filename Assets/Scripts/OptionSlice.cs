using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpawnController))]
public class OptionSlice : MonoBehaviour
{
    [SerializeField] private bool reset;
    [SerializeField] private bool play;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private SpawnController[] fruits;

    void Update()
    {
        this.transform.rotation = Quaternion.Euler(0, Time.time * rotationSpeed, 0);    
    }

    public IEnumerator Activate()
    {
        StopCoroutine(Activate());

        if (reset)
        {
            foreach (SpawnController fruit in fruits)
            {
                fruit.Reset();
            }
        }

        yield return new WaitForSeconds(3.0f);
        GetComponent<SpawnController>().Reset();
    }
}
