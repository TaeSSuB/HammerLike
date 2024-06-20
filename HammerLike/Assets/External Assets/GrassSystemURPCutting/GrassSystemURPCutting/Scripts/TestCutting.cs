using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestCutting : MonoBehaviour
{
    [SerializeField]
    GrassComputeScript grassComputeScript;

    private B_Player player;

    [SerializeField]
    float radius = 1f;

    public bool updateCuts;
    Vector3 cachedPos;
    // Start is called before the first frame update

    private void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        if (updateCuts && transform.position != cachedPos&& Input.GetMouseButtonUp(0))
        {

            grassComputeScript.UpdateCutBuffer(transform.position, radius);
            cachedPos = transform.position;

        }

       
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
