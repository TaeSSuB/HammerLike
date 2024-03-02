using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;

    public GameObject projectilePrefab; // 투사체 프리팹
    private Queue<GameObject> projectiles = new Queue<GameObject>(); // 투사체 풀

    private void Awake()
    {
        // 싱글톤 패턴으로 인스턴스가 하나만 존재하도록 보장
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 풀에서 투사체를 가져오는 메소드
    public GameObject GetProjectile()
    {
        if (projectiles.Count == 0)
        {
            AddProjectiles(1); // 사용할 투사체가 없으면 새로 생성
        }

        return projectiles.Dequeue(); // 큐에서 투사체를 제거하고 반환
    }

    // 투사체를 풀로 반환하는 메소드
    public void ReturnProjectile(GameObject projectile)
    {
        projectile.SetActive(false); // 투사체를 비활성화
        projectiles.Enqueue(projectile); // 풀에 다시 추가
    }

    // 풀에 투사체를 추가하는 메소드
    private void AddProjectiles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var newProjectile = Instantiate(projectilePrefab); // 새 투사체 생성
            newProjectile.SetActive(false); // 처음에는 비활성화
            projectiles.Enqueue(newProjectile); // 풀에 추가
            newProjectile.transform.SetParent(transform); // 선택적: 계층 구조에서 이 객체 아래에 투사체를 정리
        }
    }
}
