using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPotal : MonoBehaviour
{
    protected Vector2 startPos;
    public Vector2 endPos;
    bool isUsed = false;

    public void Teleport(GameObject obj, Vector2 position)
    {
        obj.transform.position = position;
        isUsed = true;
    }

    private void OnTriggerEnter2D(Collider2D col2d)
    {
        if(col2d.CompareTag("Player") && !isUsed)
        {
            Teleport(col2d.gameObject, endPos);
        }
    }

    private void OnTriggerExit2D(Collider2D col2d)
    {
        if (col2d.CompareTag("Player"))
        {
            isUsed = false;
        }
    }
}
