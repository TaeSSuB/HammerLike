using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAtk : MonoBehaviour
{

	public Player player;
    public Collider weaponCollider; // 공격용 콜라이더
    public float curCharging;
    public float attackDamage = 10f;
    private int attackId = 0;
    public float forceMagnitude = 500f; // AddForce에 사용할 힘의 크기
    private void Awake()
    {
        player = GetComponent<Player>();
        GameObject weaponObject = GameObject.FindGameObjectWithTag("WeaponCollider");
        if (weaponObject != null)
        {
            weaponCollider = weaponObject.GetComponent<Collider>();
            WeaponColliderScript weaponScript = weaponObject.GetComponent<WeaponColliderScript>();
            if (weaponScript == null) // WeaponColliderScript가 없다면 추가
            {
                weaponScript = weaponObject.AddComponent<WeaponColliderScript>();
            }
            weaponScript.playerAtk = this; // 현재 스크립트의 참조를 전달
        }
        else
        {
            Debug.LogError("Weapon 오브젝트를 찾을 수 없거나 Weapon 태그가 지정되어 있지 않습니다.");
        }

        // Collider를 찾았는지 확인하고, 찾지 못했다면 오류 메시지를 로그합니다.
        if (weaponCollider == null)
        {
            Debug.LogError("Weapon 오브젝트에 Collider 컴포넌트가 없습니다.");
        }
        else
        {
            weaponCollider.enabled = false; // 기본적으로 Collider를 비활성화합니다.
            Debug.Log("Weapon 오브젝트에 Collider 컴포넌트를 찾았습니다.");
        }
    }

    public void Attack()
    {
        if (Input.GetMouseButton(1))
        {
            curCharging += Time.deltaTime;
            //Debug.Log("우측키 누름");
        }
        if (Input.GetMouseButtonUp(1))
        {
            player.animCtrl.SetTrigger("tAtk");
            player.animCtrl.SetFloat("fAtkVal", curCharging);

            //Debug.Log("우측키 땜");

        }
       
        attackId++; // 새로운 공격에 대해 ID를 증가시킵니다.
        weaponCollider.gameObject.SendMessage("SetAttackId", attackId);
        //StartCoroutine(DisableWeaponColliderAfterAnimation());


    }
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster")) // "Enemy"는 충돌 대상 태그, 필요에 따라 수정
        {
            Rigidbody enemyRb = other.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                // Player가 바라보는 방향으로 힘을 가함
                Vector3 forceDirection = player.transform.forward;
                enemyRb.AddForce(forceDirection.normalized * forceMagnitude, ForceMode.Impulse);
            }
        }
    }*/
    private IEnumerator DisableWeaponColliderAfterAnimation()
    {
        // 공격 애니메이션의 길이를 가져오기 위해 현재 재생 중인 애니메이션 클립의 정보가 필요합니다.
        // 이 값은 애니메이션 클립에 따라 다를 수 있으므로 적절히 조정해야 합니다.
        float attackAnimationTime = 1.0f; // 예를 들어 공격 애니메이션이 1초간 지속된다고 가정합니다.
        yield return new WaitForSeconds(attackAnimationTime);
        weaponCollider.enabled = false; // 애니메이션이 끝나면 콜라이더를 비활성화합니다.
    }

    /*public void EnableWeaponCollider()
    {
        weaponCollider.enabled = true;
        Debug.Log("enable Weapon");
    }


    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
        Debug.Log("disable Weapon");
    }*/

}


 