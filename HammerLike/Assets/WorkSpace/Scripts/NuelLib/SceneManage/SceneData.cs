using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

 [System.Serializable]
public class SceneLink
{
    public string sceneTopic;
    public string sceneRealName;
}

[CreateAssetMenu(fileName = "SceneData", menuName = "HyunUtils/SceneData", order = 0)]
public class SceneData : ScriptableObject 
{
    public List<SceneLink> sceneDataList = new List<SceneLink>();
}
