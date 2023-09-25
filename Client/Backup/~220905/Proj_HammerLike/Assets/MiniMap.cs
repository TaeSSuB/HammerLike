using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public GameObject roomUI;
    public GameObject corridorUI;
    public GameObject playerPosUI;
    public GameObject bossPosUI;
    GameObject miniMapObj;
    GameObject bossPosUIInstance;
    GameObject playerObj;
    GameObject playerPosUIInstance;
    CRoomTemplates roomTemplate;

    bool isActive = false;

    // Start is called before the first frame update
    void Awake()
    {
        if(miniMapObj == null)
            miniMapObj = GameObject.Find("MiniMap");
        miniMapObj.SetActive(false);
        playerObj = GameObject.FindGameObjectWithTag("Player");
        roomTemplate = GameObject.FindGameObjectWithTag("Rooms").GetComponent<CRoomTemplates>();

        Invoke("GenMiniMap", 3f);
    }

    void GenMiniMap()
    {
        foreach (var room in roomTemplate.rooms)
        {
            var uiInstance = Instantiate(roomUI, miniMapObj.transform);
            uiInstance.transform.localPosition = room.transform.position * 0.5f;

            var matches = room.GetComponent<RoomSpawnManager>().matches;

            for (int i = 0; i < matches.Length; i++)
            {
                if(matches[i].isActive)
                {
                    var corridorUIInstance = Instantiate(corridorUI, miniMapObj.transform);
                    corridorUIInstance.transform.localPosition = (matches[i].roomInstance.transform.position + room.transform.position) / 2 * 0.5f;
                }
            }
        }

        playerPosUIInstance = Instantiate(playerPosUI, miniMapObj.transform);
        playerPosUIInstance.transform.localPosition = playerObj.transform.position * 0.5f;

        bossPosUIInstance = Instantiate(bossPosUI, miniMapObj.transform);
        if (roomTemplate.bossRoomInstance != null)
            bossPosUIInstance.transform.localPosition = roomTemplate.bossRoomInstance.transform.position * 0.5f;
        else
            bossPosUIInstance.transform.localPosition = roomTemplate.rooms[roomTemplate.rooms.Count - 1].transform.position * 0.5f;

        //StartCoroutine(UpdatePlayerPos());
    }

    public void VisualizeMiniMap()
    {
        if (!isActive)
        {
            isActive = true;
            miniMapObj.SetActive(true);
            StartCoroutine(UpdatePlayerPos());
        }
        else
        {
            isActive = false;
            miniMapObj.SetActive(false);
        }
    }

    IEnumerator UpdatePlayerPos()
    {
        while (isActive)
        {
            if (playerPosUIInstance != null)
            {
                playerPosUIInstance.transform.localPosition = playerObj.transform.position * 0.5f;
            }
            Debug.Log("active");
            yield return new WaitForSeconds(0.5f);
        }
    }
}
