using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛 생성 테스트용 스크립트
/// Index 값을 받아서 해당 인덱스의 유닛을 생성합니다.
/// </summary>
public class Temp_UnitSwitcher : MonoBehaviour
{
    private UnitManager unitManager;

    [SerializeField] private int unitIndex;
    // Start is called before the first frame update
    void Start()
    {
        unitManager = UnitManager.Instance;
        
        unitManager.CreateUnit(unitIndex, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
