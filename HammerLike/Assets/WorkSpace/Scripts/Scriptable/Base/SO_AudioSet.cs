using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New AudioSet", menuName = "B_ScriptableObjects/Audio/AudioSet")]
public class SO_AudioSet : ScriptableObject
{
    [System.Serializable]
    public class AudioInfo
    {
        public AudioClip clip;
        public AudioCategory category;
        public List<AudioTag> tags;
        public float volume = 1.0f;
        public bool loop = false;
    }

    public List<AudioInfo> audioInfos;
}
