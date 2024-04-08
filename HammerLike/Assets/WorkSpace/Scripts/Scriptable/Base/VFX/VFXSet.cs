using UnityEngine;

[System.Serializable]
public class VFXData
{
    public string name;
    public GameObject vfxPrefab;
    public float duration = 2f; // VFX 지속 시간, 필요에 따라 조절
}

[CreateAssetMenu(fileName = "New VFXSet", menuName = "VFX/VFXSet")]
public class VFXSet : ScriptableObject
{
    public VFXData[] vfxDatas;
}
