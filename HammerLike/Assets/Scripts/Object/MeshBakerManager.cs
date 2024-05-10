using UnityEngine;
using System.Collections.Generic;
using DigitalOpus.MB.Core;

public class MeshBakerManager : MonoBehaviour
{
    public MB3_MeshBaker meshBaker;

    void Start()
    {
        meshBaker = GetComponent<MB3_MeshBaker>();
    }

    public void BakeMeshes()
    {
        meshBaker.AddDeleteGameObjects(meshBaker.GetObjectsToCombine().ToArray(), null, true);
        meshBaker.Apply();
    }
}
