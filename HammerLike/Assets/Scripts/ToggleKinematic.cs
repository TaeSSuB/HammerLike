using UnityEngine;

public class ToggleKinematic : MonoBehaviour
{

    void Update()
    {
     
        if (Input.GetKeyDown(KeyCode.Space))
        {
           
            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = !rb.isKinematic;
            }
        }
    }
}
