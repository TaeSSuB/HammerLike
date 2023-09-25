using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[System.Serializable]
class SaveData
{
    public int intData;
    public float floatData;
    public bool boolData;
    public string strData;
}

public class DataManager : MonoBehaviour
{
    static DataManager instance;
    public static DataManager GetDataInstance() { return instance; }

    public int currentGold;

    public int clearLevelData;
    public float floatSaveData;
    public bool boolSaveData;
    public string strSaveData;

    public List<CWeapon> saveWeapons;

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        if(instance == null)
        {
            // 게임 매니저 싱글톤 패턴 구현
            // 게임 컨트롤러 유일 여부 체크
            var dm = GameObject.FindGameObjectWithTag("GameController");

            if(dm == null)
            {
                dm = new GameObject { name = "GameManager" };
                dm.tag = "GameController";
            }
            if(dm.GetComponent<DataManager>() == null)
            {
                dm.AddComponent<DataManager>();
            }
#if UNITY_EDITOR
            if (dm.GetComponent<Debugger>() == null)
            {
                dm.AddComponent<Debugger>();
            }
#endif

            DontDestroyOnLoad(dm);

            instance = dm.GetComponent<DataManager>();
        }
        else
        {
            if (this != instance)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void UseGold(int uses)
    {
        if (currentGold - uses >= 0)
            currentGold -= uses;
        else
            Debug.LogWarning("Gold is not Enough");
    }

    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
                     + "/MySaveData.dat");
        SaveData data = new SaveData();
        data.intData = clearLevelData;
        data.floatData = floatSaveData;
        data.boolData = boolSaveData;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath
                       + "/MySaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                       File.Open(Application.persistentDataPath
                       + "/MySaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            clearLevelData = data.intData;
            floatSaveData = data.floatData;
            boolSaveData = data.boolData;
            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
    }

    public void ResetData()
    {
        if (File.Exists(Application.persistentDataPath
                      + "/MySaveData.dat"))
        {
            File.Delete(Application.persistentDataPath
                              + "/MySaveData.dat");
            clearLevelData = 0;
            floatSaveData = 0.0f;
            boolSaveData = false;
            Debug.Log("Data reset complete!");
        }
        else
            Debug.LogError("No save data to delete.");
    }
}
