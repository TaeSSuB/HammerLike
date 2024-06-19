using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New AudioSet", menuName = "B_ScriptableObjects/Audio/AudioSet")]
public class SO_AudioSet : ScriptableObject
{
    [System.Serializable]
    public class AudioInfo
    {
        public string name;
        public AudioClip clip;
        public AudioCategory category;
        public List<AudioTag> tags;
        [Range(0.0f, 1.0f)]
        public float volume = 1.0f;
        [Range(0.0f, 1.0f)]
        public float initialVolume = 1.0f;
        public bool loop = false;
    }

    public List<AudioInfo> audioInfos;
}
