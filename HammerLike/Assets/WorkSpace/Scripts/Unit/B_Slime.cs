using System.Collections;
using UnityEngine;

public class B_Slime : B_Enemy
{
    [Header("Slime")]
    [SerializeField] private Collider weaponCollider;
    public float attackDistanceXMultiplier = 2f;
    public float attackDistanceYMultiplier = 1f;
    public float deadYpos = 2f;

    public override void Init()
    {
        base.Init();

        weaponCollider.enabled = false;
    }

    protected override void Dead(bool isSelf = false)
    {
        base.Dead(isSelf);
        
        weaponCollider.enabled = false;
        B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.SlimeDeath);
        StartCoroutine(CoSlimeDead());
    }

    public override void Attack()
    {
        base.Attack();

        StartCoroutine(CoSlimeAttack());
    }

    public override void StartAttack()
    {
        base.StartAttack();
        
        //col.enabled = false;
        weaponCollider.enabled = true;
        agent.enabled = false;
        //Rigid.isKinematic = true;
    }

    public override void EndAttack()
    {
        base.EndAttack();

        col.enabled = true;
        weaponCollider.enabled = false;
        agent.enabled = true;
        //Rigid.isKinematic = false;
    }
    IEnumerator CoSlimeAttack()
    {
        var lastTargetPos = GameManager.Instance.Player.transform.position;

        // 타겟까지의 거리 계산
        float target_Distance = Vector3.Distance(transform.position, lastTargetPos);

        // 초기 위치에서 타겟으로 향하는 방향 벡터 계산
        Vector3 dir = (lastTargetPos - transform.position).normalized;

        // 발사 각도와 초기 속도 계산
        float angle = 60.0f; // 발사 각도 (필요에 따라 조정)
        float projectile_Velocity = Mathf.Sqrt(target_Distance * 9.8f / Mathf.Sin(2 * angle * Mathf.Deg2Rad));

        // X와 Y 성분의 초기 속도 계산
        float Vx = projectile_Velocity * Mathf.Cos(angle * Mathf.Deg2Rad) * attackDistanceXMultiplier;
        float Vy = projectile_Velocity * Mathf.Sin(angle * Mathf.Deg2Rad) * attackDistanceYMultiplier;

        //rigid.isKinematic = false;
        Rigid.velocity = Vector3.zero;

        transform.LookAt(lastTargetPos);

        // 짧은 시간 지연
        float delayTime = 0.5f;
        yield return new WaitForSeconds(delayTime);

        // 공격 시작
        StartAttack();

        // Rigidbody에 초기 속도 적용
        Rigid.velocity = dir * Vx + Vector3.up * Vy;

        Debug.Log(Rigid.velocity);

        // 바닥에 착지할 때까지 대기
        while (!(Rigid.velocity.y <= 0 && isGrounded))
        {
            yield return null;
        }

        unitStatus.currentAttackCooltime = unitStatus.maxAttackCooltime;
        
        weaponCollider.enabled = false;

        yield return new WaitForSeconds(delayTime);

        // 공격 종료
        EndAttack();
    }

    IEnumerator CoSlimeDead()
    {
        float duration = 1f;

        while (duration > 0f)
        {
            transform.Translate(0, -deadYpos * Time.deltaTime, 0);

            duration -= Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, -5f, transform.position.z);
    }
}
