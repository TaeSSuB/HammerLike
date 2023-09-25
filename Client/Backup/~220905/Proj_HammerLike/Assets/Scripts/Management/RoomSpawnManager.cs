using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CorridorSet
{
    public bool isActive = false;
    public RoomSpawner spawner;
    public GameObject roomInstance;
    public GameObject wall;
    public GameObject potal;
    public Vector2 offset;
}

public class RoomSpawnManager : MonoBehaviour
{
    public CorridorSet[] matches = new CorridorSet[4];
    public Sprite roomSprite;
    public Sprite secretSprite;
    public int roomSpreadNum = 0;

    // Start is called before the first frame update
    void Awake()
    {
        var templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<CRoomTemplates>();

        for (int i = 0; i < matches.Length; i++)
        {
            if (matches[i].wall.transform.localPosition.x > 0)
            {
                matches[i].wall.transform.localPosition = new Vector3Int((int)(matches[i].spawner.room.gridSize.x * 0.5f + matches[i].offset.x), 0);
                matches[i].potal.GetComponent<RoomPotal>().endPos
                    = (Vector2)matches[i].wall.transform.position 
                    + (new Vector2(matches[i].spawner.room.gridSize.x, 0f) * (templates.roomPosMultiply - 1))
                    + new Vector2(3, 0)
                    + matches[i].offset;
            }
            else if (matches[i].wall.transform.localPosition.x < 0)
            {
                matches[i].wall.transform.localPosition = new Vector3Int((int)(matches[i].spawner.room.gridSize.x * -0.5f + matches[i].offset.x), 0);

                matches[i].potal.GetComponent<RoomPotal>().endPos 
                    = (Vector2)matches[i].wall.transform.position 
                    + (new Vector2(matches[i].spawner.room.gridSize.x, 0f) * -(templates.roomPosMultiply - 1))
                    + new Vector2(-3, 0)
                    + matches[i].offset;
            }
            else if (matches[i].wall.transform.localPosition.y > 0)
            {
                matches[i].wall.transform.localPosition = new Vector3Int(0, (int)(matches[i].spawner.room.gridSize.y * 0.5f + matches[i].offset.y));

                matches[i].potal.GetComponent<RoomPotal>().endPos 
                    = (Vector2)matches[i].wall.transform.position 
                    + (new Vector2(0f, matches[i].spawner.room.gridSize.y) * (templates.roomPosMultiply - 1))
                    + new Vector2(0, 3)
                    + matches[i].offset;
            }
            else if (matches[i].wall.transform.localPosition.y < 0)
            {
                matches[i].wall.transform.localPosition = new Vector3Int(0, (int)(matches[i].spawner.room.gridSize.y * -0.5f + matches[i].offset.y));
                matches[i].potal.GetComponent<RoomPotal>().endPos 
                    = (Vector2)matches[i].wall.transform.position 
                    + (new Vector2(0f, matches[i].spawner.room.gridSize.y) * -(templates.roomPosMultiply - 1))
                    + new Vector2(0, -3)
                    + matches[i].offset;
            }

            if (matches[i].isActive)
            {
                openRoomCorridor(i);
            }
            else
            {
                closeRoomCorridor(i);
            }
        }

    }

    void RandomSpawn()
    {
        for (int i = 0; i < matches.Length; i++)
        {
            matches[i].isActive = Random.Range(0, 1) <= 0 ? false : true;

            if (i == matches.Length - 1)
            {
                int idx = Random.Range(0, matches.Length - 1);
                matches[idx].isActive = true;
            }
        }
    }

    public void closeRoomCorridor(int idx)
    {
        matches[idx].spawner.gameObject.SetActive(false);

        var renderers = matches[idx].wall.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].sprite = secretSprite;

        matches[idx].wall.SetActive(true);
        matches[idx].potal.SetActive(false);
    }

    public void openRoomCorridor(int idx)
    {
        matches[idx].spawner.gameObject.SetActive(true);

        var renderers = matches[idx].wall.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].sprite = roomSprite;

        matches[idx].wall.SetActive(false);
        matches[idx].potal.SetActive(true);
    }

    

}
