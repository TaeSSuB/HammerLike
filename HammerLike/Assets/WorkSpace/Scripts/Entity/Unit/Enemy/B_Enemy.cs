using System;
using UnityEngine;

/// <summary>
/// B_Enemy : 적 유닛 클래스
/// - 모든 적 유닛은 AI를 가진다고 가정, AIStateManager 필수
/// </summary>
[RequireComponent(typeof(AIStateManager))]
public class B_Enemy : B_UnitBase
{
    private AIStateManager aIStateManager;

    // Temp - HP UI
    [Header("HP UI")]
    [SerializeField] Transform hpPosTR;

    // get aIStateManager
    public AIStateManager AIStateManager => aIStateManager;

    public static event Action<Vector3> OnMonsterDeath;

    #region Unity Callbacks & Init
    public override void Init()
    {
        base.Init();

        if(hpPosTR == null)
            hpPosTR = transform;

        FindObjectOfType<B_UIManager>().CreateHPWorldUI(hpPosTR, this);

        aIStateManager = GetComponent<AIStateManager>();
    }

    protected override void Update()
    {
        base.Update();

        // Temp - Set Chasing
        if (aIStateManager?.CurrentStateType != AIStateType.HIT && aIStateManager?.CurrentStateType != AIStateType.DEAD)
        {
            var moveDir = GameManager.Instance.Player.transform.position - transform.position;

            float applyCoordScale = GameManager.Instance.CalcCoordScale(moveDir);
            var targetDis = moveDir.magnitude / applyCoordScale;

            if (targetDis <= UnitStatus.detectRange)
            {
                if (targetDis <= UnitStatus.atkRange)
                {
                    aIStateManager?.SetState(AIStateType.ATTACK);
                }
                else
                {
                    if(!IsAttacking)
                        aIStateManager?.SetState(AIStateType.CHASE);
                }
            }
            else
            {
                aIStateManager?.SetState(AIStateType.IDLE);
            }

        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("WeaponCollider") && UnitStatus.currentHP > 0)
        {
            if(isInvincible) return;

            // Get player
            // 현재는 B_Player지만, B_UnitBase를 상속받는 모든 유닛에게 적용 가능. a.HG
            B_Player player = other.GetComponentInParent<B_Player>();

            if(player == null) return;
            
            // Get hit dir from player
            Vector3 hitDir = (transform.position - player.transform.position).normalized;

            var chargeAmount = (player.UnitStatus as SO_PlayerStatus).chargeRate;
            //var chargeAmount = (float)(player.UnitStatus.atkDamage / 10); // Temp. Magic Number
            var weaponTypeName = player.WeaponData.weaponType.ToString();

            if((player.UnitStatus as SO_PlayerStatus).maxChargeRate <= chargeAmount)
            {
                TakeDamage(hitDir, player.UnitStatus.atkDamage, player.UnitStatus.knockbackPower * chargeAmount);
            }
            else
            {
                if (player.WeaponData != null)
                {
                    player.WeaponData.WeaponAttack(transform.position, transform);
                }
                TakeDamage(hitDir, player.UnitStatus.atkDamage, player.UnitStatus.knockbackPower * chargeAmount, true);
            }

            B_AudioManager.Instance.PlaySound("Hit_" + weaponTypeName, AudioCategory.SFX);
            
            var vfxPos = other.ClosestPointOnBounds(transform.position);
            B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, vfxPos);
            

            if (RunTimeStatus.currentHP > 0)
            {
                //B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Battle);
                
                // Temp - a.HG : Hit Sound 임시 할당..
                B_AudioManager.Instance.PlaySound("Hit_" + weaponTypeName, AudioCategory.SFX);
                CameraManager.Instance.ShakeCamera();
            }
            // else
            // {
            //     B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Death);
            // }

            //ChangeState(new ChaseState(this));
            if (aIStateManager?.CurrentStateType != AIStateType.HIT && aIStateManager?.CurrentStateType != AIStateType.DEAD)
                aIStateManager?.SetState(AIStateType.HIT);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        // When hit Other Enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            CheckDead();
            
            if (IsKnockback) return;

            var other = collision.gameObject.GetComponent<B_Enemy>();
            
            if (other == null) return;
            if (!other.IsKnockback) return;

            // Get hit dir from another enemy
            Vector3 hitDir = (transform.position - collision.transform.position).normalized;

            Vector3 reflection = Vector3.Reflect(collision.relativeVelocity, hitDir);
            
            TakeDamage(hitDir, (int)(other.Rigid.mass / 2f), other.Rigid.mass);
            other.TakeDamage(reflection, (int)(other.Rigid.mass / 2f), other.Rigid.mass);

            var vfxPos = collision.contacts[0].point;
            B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, vfxPos);

            if (UnitStatus.currentHP > 0)
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Battle);
            }
            // else
            // {
            //     B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Death);
            // }

            if (aIStateManager?.CurrentStateType != AIStateType.HIT && aIStateManager?.CurrentStateType != AIStateType.DEAD)
                aIStateManager?.SetState(AIStateType.HIT);
        }

        // When hit Wall Layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            // Check Knockbacking
            if (!IsKnockback) return;
            if(Rigid.velocity.magnitude < 10f) return;

            // Get hit dir from wall
            // 입사각 & 반사각 ex.당구
            Vector3 collisionNormal = collision.contacts[0].normal;
            
            float angle = Vector3.Angle(-collision.relativeVelocity, collisionNormal);

            Vector3 reflection = Vector3.Reflect(-collision.relativeVelocity, collisionNormal);

            // Debug.Log("Debug Velocity : " + Rigid.velocity);
            // Debug.Log("Debug NormalVector : " + collisionNormal);
            // Debug.Log("Debug WallRelative : " + -collision.relativeVelocity);
            // Debug.Log("Debug Angle : " + angle);
            // Debug.Log("Debug Reflecft : " + reflection);

            Debug.DrawLine(transform.position, transform.position + Rigid.velocity.normalized * 10f, Color.white, 5f);
            Debug.DrawLine(transform.position, transform.position + collisionNormal * 10f, Color.red, 5f);
            Debug.DrawLine(transform.position, transform.position + -collision.relativeVelocity.normalized * 10f, Color.cyan, 5f);
            Debug.DrawLine(transform.position, transform.position + reflection.normalized * 10f, Color.green, 5f);

            // 데미지 강제 적용
            // 넉백 시 무적 판정이기에, 이를 무시하고 데미지를 적용
            TakeDamage(reflection, (int)Rigid.mass, Rigid.velocity.magnitude * 2f, true, false, true);

            var vfxPos = collision.contacts[0].point;
            B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, vfxPos);

            if (UnitStatus.currentHP > 0)
            {
                //B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Battle);
                B_AudioManager.Instance.PlaySound("Hit_" + "Wall", AudioCategory.SFX);
            }
            // else
            // {
            //     B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Death);
            // }

            if (aIStateManager?.CurrentStateType != AIStateType.HIT && aIStateManager?.CurrentStateType != AIStateType.DEAD)
                aIStateManager?.SetState(AIStateType.HIT);
        }
    }
    #endregion
    
    #region Check or Update State
    protected override void Dead(bool isSelf = false)
    {
        base.Dead(isSelf);
        OnMonsterDeath?.Invoke(transform.position);
        aIStateManager.SetState(AIStateType.DEAD);

        //DisconnectMusclesRecursive(GameManager.Instance.Player.transform.position);
        //Invoke(nameof(DisconnectMusclesRecursive), 0.1f);
    }
    #endregion
    
    #region Action
    public override void Attack()
    {
        base.Attack();

        //transform.LookAt(GameManager.Instance.Player.transform);
    }    
    #endregion

    #region Animation Event
    public override void OnStartAttack()
    {
        base.OnStartAttack();
    }

    public override void OnEndAttack()
    {
        base.OnEndAttack();
    }
    #endregion

}
