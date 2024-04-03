using UnityEngine;

[System.Serializable]
public class VFXData
{
    public string name;
    public GameObject vfxPrefab;
    public float duration = 2f; // VFX ���� �ð�, �ʿ信 ���� ����
}

[CreateAssetMenu(fileName = "New VFXSet", menuName = "VFX/VFXSet")]
public class VFXSet : ScriptableObject
{
    public VFXData[] vfxDatas;
}
