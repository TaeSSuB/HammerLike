using RootMotion.Demos;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshDestroy : MonoBehaviour
{
    private bool edgeSet = false;
    private Vector3 edgeVertex = Vector3.zero;
    private Vector2 edgeUV = Vector2.zero;
    private Plane edgePlane = new Plane();
    public Player player;
    private bool hasBeenDestroyed = false;
    public int CutCascades = 1;
    public float ExplodeForce = 0;
    private HashSet<int> processedAttacks = new HashSet<int>();
    //public Collider meshCol;
    //public Collider weaponCollider;
    public float deathTime = 1.0f;
    public float maxHp = 50f;
    public float curHp;
    private int lastProcessedAttackId = -1;
    private Collider collider;
    private ItemManager itemManager;
    private ChangeMaterialObject changeMaterial;
    private CameraManager cameraShake;
    void Start()
    {
        GameObject itemDB = GameObject.Find("ItemDB");
        
        // Temp 240426 - 주석처리 a.HG
        //itemManager = itemDB.GetComponent<ItemManager>(); 

        curHp = maxHp;
        changeMaterial = GetComponent<ChangeMaterialObject>(); 
        // player가 null인 경우 Player 이름을 가진 오브젝트에서 Player 스크립트를 찾아 할당
        //if (player == null)
        //{
        //    GameObject playerObject = GameObject.Find("Player");
        //    if (playerObject != null) // Player 오브젝트가 존재하는 경우
        //    {
        //        player = playerObject.GetComponent<Player>();
        //        if (player == null) // Player 스크립트가 해당 오브젝트에 없는 경우
        //        {
        //            Debug.LogError("Player 오브젝트에서 Player 스크립트를 찾을 수 없습니다.");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Scene 내에 Player 이름을 가진 오브젝트가 존재하지 않습니다.");
        //    }
        //}

        //GameObject vcamObject = GameObject.Find("CM vcam1");
        //if (vcamObject != null) // CM vcam1 오브젝트가 존재하는 경우
        //{
        //    cameraShake = vcamObject.GetComponent<CameraShake>();
        //    if (cameraShake == null) // CameraShake 컴포넌트가 해당 오브젝트에 없는 경우
        //    {
        //        Debug.LogError("CM vcam1 오브젝트에서 CameraShake 컴포넌트를 찾을 수 없습니다.");
        //    }
        //}
        //else
        //{
        //    Debug.LogError("Scene 내에 CM vcam1 이름을 가진 오브젝트가 존재하지 않습니다.");
        //}

    }

    void Update()
    {
       /*if(Input.GetKeyDown(KeyCode.T))
        {
            DestroyMesh();
        }*/

        /*if (meshCol != null && weaponCollider != null && weaponCollider.enabled && meshCol.bounds.Intersects(weaponCollider.bounds))
        {
            DestroyMesh();
        }*/
    }
    private void OnCollisionEnter(Collision other)
    {
        //DestroyMesh();
        if (other.gameObject.CompareTag("Skeleton_Prisoner") || other.gameObject.CompareTag("Skeleton_Archer"))
        {
            Monster monster = other.gameObject.GetComponent<Monster>();
            if (monster != null && monster.isKnockedBack)
            {
                TakeDamage(monster.knockbackDamage);
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        /*if(other.tag =="Skeleton_Prisoner")
        {
            DestroyMesh();
        }*/

        if (other.CompareTag("WeaponCollider"))
        {
            WeaponCollider weaponCollider = other.GetComponent<WeaponCollider>();
            if (weaponCollider != null && lastProcessedAttackId != weaponCollider.CurrentAttackId)
            {
                // ??¹??? ¹× ³?¹? ?³¸®
                PlayerAtk playerAttack = other.GetComponentInParent<PlayerAtk>();
                if (playerAttack != null)
                {
                    //TakeDamage(playerAttack.attackDamage);
                    //raycastShooter.ShootAtBone(targetBones[0]);
                    /*Vector3 hitPoint = other.ClosestPointOnBounds(transform.position); // 충돌 지점
                    Vector3 knockbackDirection = DetermineKnockbackDirection(hitPoint, other.transform);
                    ApplyKnockback(knockbackDirection);*/
                    //lastProcessedAttackId = weaponCollider.CurrentAttackId;
                    //processedAttacks.Add(weaponCollider.CurrentAttackId);
                    float customDamage = 0f;

                    if (player.atk.curCharging >= 0 && player.atk.curCharging < 2)
                    {
                        customDamage = playerAttack.attackDamage;
                    }
                    else if (player.atk.curCharging >= 2 && player.atk.curCharging < 4)
                    {
                        customDamage = playerAttack.attackDamage * 1.5f;
                    }
                    else // player.curCharging >= 4
                    {
                        customDamage = playerAttack.attackDamage * 2f;
                    }
                    TakeDamage(customDamage);
                    //raycastShooter.ShootAtBoneWithForce(targetBones[0], customForce);
                    //raycastShooter.ShootAtBone(targetBones[0]);
                    lastProcessedAttackId = weaponCollider.CurrentAttackId;
                    weaponCollider.hasProcessedAttack = true; // 공격 처리 표시
                }

            }
        }
        
    }

    private void TakeDamage(float damage)
    {
        if (curHp <= 0) return; // ??¹? ??¸??? °æ¿? ??¹???¸? ¹Þ?? ¾??½

        if (curHp > 0)  // ¸?½º??°¡ ??¾Æ???? ¶§¸¸ ??°? ?³¸®
        {
            curHp -= damage;
            if (!CompareTag("Pot"))
            {
                cameraShake.ShakeCamera();
            }

            if (curHp <= 0)
            {
                DestroyMesh();
            }
            else
            {
                changeMaterial.OnHit();
            }
        }
    }

    public void DestroyMesh()
    {
        if (hasBeenDestroyed) // 이미 DestroyMesh가 호출되었으면 아무 것도 하지 않음
            return;

        hasBeenDestroyed = true;
        if (CompareTag("Pot"))
            itemManager.DropItem(0,transform.position);
        else
        {
            
        }
        // 여기서 null 체크를 추가합니다.
        if (GetComponent<MeshFilter>() == null || player == null)
        {
            Debug.LogError("MeshFilter 또는 Player가 null입니다.");
            return;
        }

        var originalMesh = GetComponent<MeshFilter>().mesh;
        originalMesh.RecalculateBounds();
        var parts = new List<PartMesh>();
        var subParts = new List<PartMesh>();

        var mainPart = new PartMesh()
        {
            UV = originalMesh.uv,
            Vertices = originalMesh.vertices,
            Normals = originalMesh.normals,
            Triangles = new int[originalMesh.subMeshCount][],
            Bounds = originalMesh.bounds
        };
        for (int i = 0; i < originalMesh.subMeshCount; i++)
            mainPart.Triangles[i] = originalMesh.GetTriangles(i);

        parts.Add(mainPart);

        for (var c = 0; c < CutCascades; c++)
        {
            for (var i = 0; i < parts.Count; i++)
            {
                var bounds = parts[i].Bounds;
                bounds.Expand(0.5f);

                var plane = new Plane(UnityEngine.Random.onUnitSphere, new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                                                                                   UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                                                                                   UnityEngine.Random.Range(bounds.min.z, bounds.max.z)));


                subParts.Add(GenerateMesh(parts[i], plane, true));
                subParts.Add(GenerateMesh(parts[i], plane, false));
            }
            parts = new List<PartMesh>(subParts);
            subParts.Clear();
        }

        Vector3 playerDirection = player.transform.forward; // 플레이어가 바라보는 방향

        for (var i = 0; i < parts.Count; i++)
        {
            parts[i].MakeGameobject(this);
            var rigidbody = parts[i].GameObject.GetComponent<Rigidbody>();

            // 플레이어가 바라보는 방향으로 파편에 힘을 가합니다.
            rigidbody.AddForce(playerDirection * ExplodeForce, ForceMode.Impulse);
            // 생성된 각 파편을 10초 후에 파괴합니다.
            Destroy(parts[i].GameObject, deathTime);
        }
        Destroy(gameObject);
        // 부모 오브젝트의 BoxCollider를 파괴합니다.
        if (transform.parent != null)
        {
            BoxCollider parentBoxCollider = transform.parent.gameObject.GetComponent<BoxCollider>();
            if (parentBoxCollider != null)
            {
                Destroy(parentBoxCollider);
                Destroy(gameObject);
            }
        }
        else
        {
            // 부모 오브젝트가 없으면 이 오브젝트 자체를 파괴합니다.
            Destroy(gameObject);
        }
    }

    private PartMesh GenerateMesh(PartMesh original, Plane plane, bool left)
    {
        var partMesh = new PartMesh() { };
        var ray1 = new Ray();
        var ray2 = new Ray();


        for (var i = 0; i < original.Triangles.Length; i++)
        {
            var triangles = original.Triangles[i];
            edgeSet = false;

            for (var j = 0; j < triangles.Length; j = j + 3)
            {
                var sideA = plane.GetSide(original.Vertices[triangles[j]]) == left;
                var sideB = plane.GetSide(original.Vertices[triangles[j + 1]]) == left;
                var sideC = plane.GetSide(original.Vertices[triangles[j + 2]]) == left;

                var sideCount = (sideA ? 1 : 0) +
                                (sideB ? 1 : 0) +
                                (sideC ? 1 : 0);
                if (sideCount == 0)
                {
                    continue;
                }
                if (sideCount == 3)
                {
                    partMesh.AddTriangle(i,
                                         original.Vertices[triangles[j]], original.Vertices[triangles[j + 1]], original.Vertices[triangles[j + 2]],
                                         original.Normals[triangles[j]], original.Normals[triangles[j + 1]], original.Normals[triangles[j + 2]],
                                         original.UV[triangles[j]], original.UV[triangles[j + 1]], original.UV[triangles[j + 2]]);
                    continue;
                }

                //컷 포인트
                var singleIndex = sideB == sideC ? 0 : sideA == sideC ? 1 : 2;

                ray1.origin = original.Vertices[triangles[j + singleIndex]];
                var dir1 = original.Vertices[triangles[j + ((singleIndex + 1) % 3)]] - original.Vertices[triangles[j + singleIndex]];
                ray1.direction = dir1;
                plane.Raycast(ray1, out var enter1);
                var lerp1 = enter1 / dir1.magnitude;

                ray2.origin = original.Vertices[triangles[j + singleIndex]];
                var dir2 = original.Vertices[triangles[j + ((singleIndex + 2) % 3)]] - original.Vertices[triangles[j + singleIndex]];
                ray2.direction = dir2;
                plane.Raycast(ray2, out var enter2);
                var lerp2 = enter2 / dir2.magnitude;

                //first vertex = ancor
                AddEdge(i,
                        partMesh,
                        left ? plane.normal * -1f : plane.normal,
                        ray1.origin + ray1.direction.normalized * enter1,
                        ray2.origin + ray2.direction.normalized * enter2,
                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));

                if (sideCount == 1)
                {
                    partMesh.AddTriangle(i,
                                        original.Vertices[triangles[j + singleIndex]],
                                        //Vector3.Lerp(originalMesh.vertices[triangles[j + singleIndex]], originalMesh.vertices[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        //Vector3.Lerp(originalMesh.vertices[triangles[j + singleIndex]], originalMesh.vertices[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                                        ray1.origin + ray1.direction.normalized * enter1,
                                        ray2.origin + ray2.direction.normalized * enter2,
                                        original.Normals[triangles[j + singleIndex]],
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                                        original.UV[triangles[j + singleIndex]],
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));

                    continue;
                }

                if (sideCount == 2)
                {
                    partMesh.AddTriangle(i,
                                        ray1.origin + ray1.direction.normalized * enter1,
                                        original.Vertices[triangles[j + ((singleIndex + 1) % 3)]],
                                        original.Vertices[triangles[j + ((singleIndex + 2) % 3)]],
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        original.Normals[triangles[j + ((singleIndex + 1) % 3)]],
                                        original.Normals[triangles[j + ((singleIndex + 2) % 3)]],
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        original.UV[triangles[j + ((singleIndex + 1) % 3)]],
                                        original.UV[triangles[j + ((singleIndex + 2) % 3)]]);
                    partMesh.AddTriangle(i,
                                        ray1.origin + ray1.direction.normalized * enter1,
                                        original.Vertices[triangles[j + ((singleIndex + 2) % 3)]],
                                        ray2.origin + ray2.direction.normalized * enter2,
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        original.Normals[triangles[j + ((singleIndex + 2) % 3)]],
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        original.UV[triangles[j + ((singleIndex + 2) % 3)]],
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));
                    continue;
                }


            }
        }

        partMesh.FillArrays();

        return partMesh;
    }

    private void AddEdge(int subMesh, PartMesh partMesh, Vector3 normal, Vector3 vertex1, Vector3 vertex2, Vector2 uv1, Vector2 uv2)
    {
        if (!edgeSet)
        {
            edgeSet = true;
            edgeVertex = vertex1;
            edgeUV = uv1;
        }
        else
        {
            edgePlane.Set3Points(edgeVertex, vertex1, vertex2);

            partMesh.AddTriangle(subMesh,
                                edgeVertex,
                                edgePlane.GetSide(edgeVertex + normal) ? vertex1 : vertex2,
                                edgePlane.GetSide(edgeVertex + normal) ? vertex2 : vertex1,
                                normal,
                                normal,
                                normal,
                                edgeUV,
                                uv1,
                                uv2);
        }
    }

    public class PartMesh
    {
        private List<Vector3> _Verticies = new List<Vector3>();
        private List<Vector3> _Normals = new List<Vector3>();
        private List<List<int>> _Triangles = new List<List<int>>();
        private List<Vector2> _UVs = new List<Vector2>();
        public Vector3[] Vertices;
        public Vector3[] Normals;
        public int[][] Triangles;
        public Vector2[] UV;
        public GameObject GameObject;
        public Bounds Bounds = new Bounds();

        public PartMesh()
        {

        }

        public void AddTriangle(int submesh, Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 normal1, Vector3 normal2, Vector3 normal3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            if (_Triangles.Count - 1 < submesh)
                _Triangles.Add(new List<int>());

            _Triangles[submesh].Add(_Verticies.Count);
            _Verticies.Add(vert1);
            _Triangles[submesh].Add(_Verticies.Count);
            _Verticies.Add(vert2);
            _Triangles[submesh].Add(_Verticies.Count);
            _Verticies.Add(vert3);
            _Normals.Add(normal1);
            _Normals.Add(normal2);
            _Normals.Add(normal3);
            _UVs.Add(uv1);
            _UVs.Add(uv2);
            _UVs.Add(uv3);

            Bounds.min = Vector3.Min(Bounds.min, vert1);
            Bounds.min = Vector3.Min(Bounds.min, vert2);
            Bounds.min = Vector3.Min(Bounds.min, vert3);
            Bounds.max = Vector3.Min(Bounds.max, vert1);
            Bounds.max = Vector3.Min(Bounds.max, vert2);
            Bounds.max = Vector3.Min(Bounds.max, vert3);
        }

        public void FillArrays()
        {
            Vertices = _Verticies.ToArray();
            Normals = _Normals.ToArray();
            UV = _UVs.ToArray();
            Triangles = new int[_Triangles.Count][];
            for (var i = 0; i < _Triangles.Count; i++)
                Triangles[i] = _Triangles[i].ToArray();
        }

        public void MakeGameobject(MeshDestroy original)
        {
            GameObject = new GameObject(original.name);
            GameObject.transform.position = original.transform.position;
            GameObject.transform.rotation = original.transform.rotation;
            GameObject.transform.localScale = original.transform.localScale;

            var mesh = new Mesh();
            mesh.name = original.GetComponent<MeshFilter>().mesh.name;

            mesh.vertices = Vertices;
            mesh.normals = Normals;
            mesh.uv = UV;
            for (var i = 0; i < Triangles.Length; i++)
                mesh.SetTriangles(Triangles[i], i, true);
            Bounds = mesh.bounds;

            var renderer = GameObject.AddComponent<MeshRenderer>();
            renderer.materials = original.GetComponent<MeshRenderer>().materials;

            var filter = GameObject.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            var collider = GameObject.AddComponent<MeshCollider>();
            collider.convex = true;

            var rigidbody = GameObject.AddComponent<Rigidbody>();
            var meshDestroy = GameObject.AddComponent<MeshDestroy>();
            meshDestroy.CutCascades = original.CutCascades;
            meshDestroy.ExplodeForce = original.ExplodeForce;

        }

    }
}