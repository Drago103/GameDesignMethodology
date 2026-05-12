using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject destination;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = new Vector3(destination.transform.position.x, destination.transform.position.y, destination.transform.position.z);
        }
    }
}
