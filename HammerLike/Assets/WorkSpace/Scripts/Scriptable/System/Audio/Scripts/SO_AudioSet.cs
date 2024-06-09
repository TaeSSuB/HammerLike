using UnityEngine;
using System.Collections.Generic;

//public enum AudioCategory { BGM, SFX }
//public enum AudioTag
//{
//    MainMenu,
//    Battle,
//    Ambient,
//    UI,
//    Death,
//    PickUp,
//    Crash,
//    Town,
//    SlimeDeath
//}

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
        public float volume = 1.0f;
        public bool loop = false;
    }

    public List<AudioInfo> audioInfos;
}
