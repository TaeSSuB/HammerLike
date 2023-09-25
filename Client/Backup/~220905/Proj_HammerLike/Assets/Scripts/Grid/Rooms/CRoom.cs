using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using hger;

public class CRoom : MonoBehaviour
{
    [Header("Grid")]
    public CPathFinder finder;
    public CGridMap gridMap;
    public Vector2Int gridSize = new Vector2Int(3, 3);
    public Vector2 gridOffset = Vector2.zero;
    public float cellSize = 1f;

    public bool allowDiagonal;
    public bool allowThrow;

    [Header("Room Manage")]
    public bool isEnterPlayer; // 플레이어 출입 판별
    public bool isClear; // 클리어 여부 판별
    public bool isAuto;
    public CUnit[] monsterArr; // 몬스터 오브젝트  배열
    public GameObject[] gimmickArr; // 기믹 오브젝트 배열
    public Transform[] spawnTransformArr; // 몬스터 스폰 위치 배열
    public Vector2[] gimmickPosArr; // 기믹 스폰 위치 배열
    public List<GameObject> spawnedMobArr;
    public RoomSpawnManager spawnManager;
    //public GameObject blackPanel;
    public Light2D roomLight2D;
    public GameObject spawnTrParent;
    public GameObject potals; // 포탈 오브젝트
    public BoxCollider2D roomCol2D;

    public void SpawnMob(CUnit mob, Vector2 spawnPos)
    {
        //var mobInst = Instantiate(mob.gameObject, spawnPos, mob.gameObject.transform.rotation);
        //mobInst.GetComponent<CUnit>().SetRoom(this);
        //spawnedMobArr.Add(mobInst);
        mob.SetRoom(this);
        spawnedMobArr.Add(Instantiate(mob.gameObject, spawnPos, mob.gameObject.transform.rotation));
        //mob.gameObject.SetActive(true);
    }

    void SpawnGimmick(GameObject gimmick)
    {
        gimmick.SetActive(true);
    }

    public void SpawnAll()
    {
        // Initiallize
        spawnedMobArr.Clear();

        if (spawnTransformArr.Length - 1 >= monsterArr.Length)
        {
            //foreach(CUnit unit in monsterArr)
            for (int i = 0; i < monsterArr.Length; i++)
            {
                SpawnMob(monsterArr[i], spawnTransformArr[i + 1].position);
            }
        }
        else
            Debug.LogError("Room's Mob Spawn Pos Length is less than Mobs");

        foreach (GameObject gimmick in gimmickArr)
            SpawnGimmick(gimmick);
    }

    public void KillAll()
    {
        foreach(GameObject mob in spawnedMobArr)
        {
            Destroy(mob);
        }
    }


    void ControlWall(bool _boolean)
    {
        if (spawnManager != null && isAuto)
        {
            for (int i = 0; i < spawnManager.matches.Length; i++)
            {
                if (spawnManager.matches[i].isActive)
                {
                    spawnManager.matches[i].wall.SetActive(_boolean);
                }
            }
        }
    }

    public void CameraMove()
    {
        var myPos = gameObject.transform.position;
        Camera.main.gameObject.transform.position = new Vector3(myPos.x, myPos.y, Camera.main.gameObject.transform.position.z);
    }

    void Awake()
    {
        gridMap = new CGridMap(gridSize.x, gridSize.y, cellSize, gridOffset + (Vector2)gameObject.transform.position);
        finder = new CPathFinder(gridMap, allowDiagonal, allowThrow);

        if (spawnManager == null && isAuto)
            spawnManager = GetComponentInParent<RoomSpawnManager>();

        if (roomCol2D == null)
            roomCol2D = GetComponent<BoxCollider2D>();
        if (roomLight2D == null)
            roomLight2D = GetComponent<Light2D>();

        roomLight2D.gameObject.transform.localScale = (Vector3Int)(new Vector2Int(gridSize.x + 1, gridSize.y + 1));
        roomLight2D.intensity = 0.02f;

        roomCol2D.size = new Vector2Int(gridSize.x - 2, gridSize.y - 2);

        if (spawnTrParent != null)
            spawnTransformArr = spawnTrParent.GetComponentsInChildren<Transform>();

        potals.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            gridMap.drawGridAll();
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log(Utils.GetMouseWorldPosition());

            int xx, yy;
            gridMap.GetXY(Utils.GetMouseWorldPosition(), out xx, out yy);

            //Debug.Log(new Vector2(xx, yy));
        }

        if (isEnterPlayer)
        {
            int remainMob = spawnedMobArr.Count;

            if (remainMob > 0)
            {
                foreach (GameObject mob in spawnedMobArr)
                {
                    if (!mob.activeSelf)
                        remainMob--;
                }
            }

            if (remainMob <= 0 && !isClear)
            {
                Debug.Log(this + ": Clear");
                ControlWall(false);
                potals.SetActive(true);
                isClear = true;
            }
        }
    }

    // 유닛 출입 인식
    private void OnTriggerEnter2D(Collider2D col2d)
    {
        if(col2d.CompareTag("Player"))
        {
            //CameraMove();
            //blackPanel.SetActive(false);

            if (!isClear && !isEnterPlayer)
            {
                isEnterPlayer = true;
                roomLight2D.intensity = 0.5f;
                Debug.Log("Player Enter : " + isEnterPlayer);

                SpawnAll();

                ControlWall(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col2d)
    {
        if (col2d.CompareTag("Player"))
        {
            //blackPanel.SetActive(true);

            if (isClear)
            {
                isEnterPlayer = false;
                //roomLight2D.intensity = 0.02f;
                Debug.Log("Player Enter : " + isEnterPlayer);
            }

            //if (!isClear)
            //{
            //    KillAll();

            //    ControlWall(false);
            //}
        }
    }

    //void OnSceneGUI()
    //{

    //}

}
