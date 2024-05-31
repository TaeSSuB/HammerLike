using UnityEngine;

/// <summary>
/// WeaponOrbit : 무기 궤도 클래스
/// </summary>
public class WeaponOrbitPlayer : TrailOrbit
{
    //[Header("References")]
    private Renderer weaponRenderer;  // 무기 렌더러
    private B_Player player;  // 플레이어 오브젝트

    void OnEnable()
    {
        if(player == null)
        {
            player = FindFirstObjectByType<B_Player>();
            //player = GameObject.FindGameObjectWithTag("Player").GetComponent<B_Player>();
            //player = GameManager.Instance.Player;
        }

        player.OnWeaponEquipped += ApplyWeapon;
    }

    void OnDisable()
    {
        player.OnWeaponEquipped -= ApplyWeapon;
    }

    public void ApplyWeapon(B_Weapon inWeapon)
    {
        if(inWeapon == null)
            return;

        targetObj = inWeapon.VFXObj;
        weaponRenderer = inWeapon.MeshRenderer;

        ApplyMaterial(inWeapon.WeaponTrailMat);
        AdjustTrailWidth(weaponRenderer);
    }

    protected override void Start()
    {
        base.Start();

        if(player == null)
        {
            player = GameManager.Instance.Player;
        }

        targetObj = player.gameObject;
        
        //ApplyWeapon(b_Weapon);
    }

    #region DEBUG
    /// <summary>
    /// TrackDirBasedPoint Based 궤도 위치와 가장 가까운 궤도 위치를 시각화
    /// </summary>
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    #endregion
    
}
