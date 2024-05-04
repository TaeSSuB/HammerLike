using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NuelLib;
using Unity.AI.Navigation;

public class B_NavManager : SingletonMonoBehaviour<B_NavManager>
{
    [Header("NavMesh")]
    [SerializeField] private NavMeshSurface navMeshSurface;
}
