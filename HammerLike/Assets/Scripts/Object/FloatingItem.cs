using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static ToonyColorsPro.ShaderGenerator.Enums;

public class FloatingItem : MonoBehaviour
{
    public ItemManager itemManager; // ItemManager 참조
    public int itemId; // 이 아이템의 ID

    private Rigidbody rb;
    private float originalY;
    private float maxY;
    private float speed = 1f;
    private bool isMovingUp = false;
    private float timeToKinematic = 2f; // Kinematic으로 전환하기 전 대기 시간
    private float timer = 0f; // 타이머
    private bool isDropped = false;
    private float obtainableDistance = 8f; // 아이템을 플레이어가 얻을 수 있는 거리

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalY = transform.position.y + 0.5f;
        maxY = originalY + 1f;
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse); // 초기 힘 추가
    }

    void Update()
    {
        // 플레이어와의 거리를 계산
        float distanceToPlayer = Vector3.Distance(itemManager.player.transform.position, transform.position);
        /// 현재는 모든 아이템이 다 자석처럼 붙게했지만 골드만 자동으로 따라오고 다른 아이템은
        /// Z키를 눌러야만 따라오게 해야한다
        /// 따라서 boolean변수를 하나 더 만들어서 획득했는지 안획득했는지를 판단하게 하는게 맞는거같다
        if (distanceToPlayer < obtainableDistance)
        {
            // 아이템을 플레이어에게 당겨옴
            rb.isKinematic = false;
            Vector3 directionToPlayer = (itemManager.player.transform.position - transform.position).normalized;
            rb.MovePosition(transform.position + directionToPlayer * 10f * Time.deltaTime);
        }

        if (isDropped && distanceToPlayer > obtainableDistance)
        {
            timer += Time.deltaTime;
            if (timer >= timeToKinematic)
            {
                rb.isKinematic = true; // 지정된 시간 이후에 isKinematic을 true로 설정
                isDropped = false; // 한 번만 설정되도록
            }
        }

        if (isMovingUp && !isDropped)
        {
            // 위로 이동
            if (transform.position.y < maxY)
            {
                float newY = Mathf.MoveTowards(transform.position.y, maxY, speed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
            else
            {
                isMovingUp = false; // 최대 높이 도달 시 이동 방향 변경
            }
        }
        else
        {
            // 원래 위치로 이동
            if (transform.position.y > originalY)
            {
                float newY = Mathf.MoveTowards(transform.position.y, originalY, speed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
            else
            {
                isMovingUp = true; // 원래 위치로 도달 시 이동 방향 변경
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Room"))
        {
            isDropped = true;
            isMovingUp = true;
            // 여기서는 isKinematic을 설정하지 않음
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어와 접촉 시 아이템을 인벤토리에 추가하고 객체 파괴
            itemManager.GiveItemToPlayer(itemId);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 플레이어와 접촉 시 아이템을 인벤토리에 추가하고 객체 파괴
            itemManager.GiveItemToPlayer(itemId);
            Destroy(gameObject);
        }
    }
}
