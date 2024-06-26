using UnityEngine;
using System.Collections.Generic;
using DigitalOpus.MB.Core;

public class MeshBakerSetup : MonoBehaviour
{
    public GameObject inputObject; // 메시를 결합할 오브젝트들의 부모 오브젝트
    public GameObject parentSceneObject; // 베이킹된 메시의 부모가 될 오브젝트

    private MB3_TextureBaker textureBaker;
    private MB3_MeshBaker meshBaker;

    void Start()
    {
        // 필요한 컴포넌트 가져오기
        textureBaker = GetComponent<MB3_TextureBaker>();
        meshBaker = GetComponentInChildren<MB3_MeshBaker>();

        if (textureBaker == null || meshBaker == null)
        {
            Debug.LogError("TextureBaker 또는 MeshBaker 컴포넌트가 필요합니다.");
            return;
        }

        // 초기 설정
        textureBaker.resultMaterial = new Material(Shader.Find("HLPixelizer/SRP/HLPixelizer")); // 필요에 따라 적절한 쉐이더 설정
        textureBaker.textureBakeResults = ScriptableObject.CreateInstance<MB2_TextureBakeResults>();

        BakeMeshes();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Bake Meshes"))
        {
            BakeMeshes();
        }
    }

    void BakeMeshes()
    {
        // 입력 오브젝트에서 모든 렌더러 컴포넌트 가져오기
        var renderers = inputObject.GetComponentsInChildren<Renderer>();
        List<GameObject> objectsToCombine = new List<GameObject>();

        foreach (var renderer in renderers)
        {
            objectsToCombine.Add(renderer.gameObject);
        }

        // 텍스처 베이커에 오브젝트들 추가
        textureBaker.objsToMesh = objectsToCombine;

        // 텍스처 아틀라스 생성
        textureBaker.CreateAtlases();

        // 메시 베이커 설정
        meshBaker.textureBakeResults = textureBaker.textureBakeResults;
        meshBaker.useObjsToMeshFromTexBaker = true;
        meshBaker.parentSceneObject = parentSceneObject.transform;

        // 메시 베이킹
        if (meshBaker.AddDeleteGameObjects(objectsToCombine.ToArray(), null, true))
        {
            meshBaker.Apply();
        }
    }
}
