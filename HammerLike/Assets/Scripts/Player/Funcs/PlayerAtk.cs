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
        StartCoroutine(DisableWeaponColliderAfterAnimation(attackId));


    }

    private IEnumerator DisableWeaponColliderAfterAnimation(int currentAttackId)
    {
        float attackAnimationTime = 1.0f; // 공격 애니메이션 시간
        yield return new WaitForSeconds(attackAnimationTime);

        // 현재 공격 ID와 저장된 공격 ID가 같고, weaponCollider가 여전히 활성화되어 있다면 비활성화
        WeaponCollider weaponColliderScript = weaponCollider.GetComponent<WeaponCollider>();
        if (weaponColliderScript != null && weaponColliderScript.CurrentAttackId == currentAttackId && weaponCollider.enabled)
        {
            weaponCollider.enabled = false;
        }
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


 