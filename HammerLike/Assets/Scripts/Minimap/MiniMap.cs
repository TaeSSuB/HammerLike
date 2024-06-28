using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Material lineMaterial;

    public void GenerateMiniMap(List<BSPNode> rooms)
    {
        foreach (var room in rooms)
        {
            if (room.roomObject != null)
            {
                DrawRoomOutline(room);
            }
        }
    }

    private void DrawRoomOutline(BSPNode room)
    {
        BoxCollider boxCollider = room.roomObject.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Vector3[] corners = GetBoxColliderCorners(boxCollider);
            CreateLineRenderer(corners);
        }
    }

    private Vector3[] GetBoxColliderCorners(BoxCollider boxCollider)
    {
        Vector3 center = boxCollider.center;
        Vector3 size = boxCollider.size;
        Vector3[] corners = new Vector3[4];

        corners[0] = boxCollider.transform.TransformPoint(center + new Vector3(-size.x, 0, -size.z) * 0.5f);
        corners[1] = boxCollider.transform.TransformPoint(center + new Vector3(size.x, 0, -size.z) * 0.5f);
        corners[2] = boxCollider.transform.TransformPoint(center + new Vector3(size.x, 0, size.z) * 0.5f);
        corners[3] = boxCollider.transform.TransformPoint(center + new Vector3(-size.x, 0, size.z) * 0.5f);

        return corners;
    }

    private void CreateLineRenderer(Vector3[] corners)
    {
        GameObject lineObject = new GameObject("MiniMapLine");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.positionCount = 5;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.SetPosition(0, corners[0]);
        lineRenderer.SetPosition(1, corners[1]);
        lineRenderer.SetPosition(2, corners[2]);
        lineRenderer.SetPosition(3, corners[3]);
        lineRenderer.SetPosition(4, corners[0]); // Close the loop
    }
}
