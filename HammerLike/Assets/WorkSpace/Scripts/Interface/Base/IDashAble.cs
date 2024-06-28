using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDashAble
{
    void Dash(Vector3 inDir);
    IEnumerator CoDash(Vector3 inDir);

    /// <summary>
    /// StartDash : 대쉬 시작
    /// moveSpeed를 대쉬 속도로 변경하고, 대쉬 상태를 고정
    /// </summary>
    void OnStartDash();
    
    /// <summary>
    /// EndDash : 대쉬 종료
    /// moveSpeed를 원래대로 돌리고, 대쉬 상태를 초기화
    /// </summary>
    void OnEndDash();
}
