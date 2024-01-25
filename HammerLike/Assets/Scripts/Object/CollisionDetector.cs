using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public FloatingItem floatingItem;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Room") && floatingItem != null)
        {
            //floatingItem.EnableFloating();
        }
    }
}
