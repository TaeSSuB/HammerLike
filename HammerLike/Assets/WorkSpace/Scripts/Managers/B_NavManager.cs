using UnityEngine;
using NuelLib;
using Unity.AI.Navigation;

/// <summary>
/// B_NavManager : 네비게이션 매니저 클래스 (Singleton MonoBehavior)
/// - NavMeshSurface의 생성 및 할당에 활용
/// - Room & Corrider 생성 이후 NavMeshSurface Baking 필요
/// </summary>
public class B_NavManager : SingletonMonoBehaviour<B_NavManager>
{
    [Header("NavMesh")]
    [SerializeField] private NavMeshSurface navMeshSurface;
    
}
