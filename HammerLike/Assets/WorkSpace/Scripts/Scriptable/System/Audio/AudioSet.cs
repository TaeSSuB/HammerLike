using UnityEngine;

[System.Serializable]
public class AudioData
{
    public string name;
    public AudioClip clip;
    public float volume = 1.0f;
    public bool loop = false;
}

[CreateAssetMenu(fileName = "New AudioSet", menuName = "Audio/AudioSet")]
public class AudioSet : ScriptableObject
{
    public AudioData[] audioDatas;
}
