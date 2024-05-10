using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IncreaseScale: 스크립트가 장착된 오브젝트의 스케일을 조정하는 스크립트.
/// 이 스크립트는 에디터 모드에서도 실행됩니다.
/// </summary>
[ExecuteInEditMode]
public class WorldScale : MonoBehaviour
{
    public SO_SystemSettings systemSettings; 
    [SerializeField] private bool editorMode = false;
    [SerializeField] private bool reset = false;

    void Start()
    {
        ApplyScale();
    }

    void Update()
    {
        if (editorMode)
        {
            ApplyScale();
        }

        if(reset)
        {
            ResetScale();
        }
    }

    private void ApplyScale()
    {
        if (systemSettings != null)
        {
            Vector3 finalScale = systemSettings.CoordinateScale; 
            transform.localScale = finalScale;
        }
    }

    private void ResetScale()
    {
        if (systemSettings != null)
        {
            Vector3 finalScale = new Vector3(1, 1, 1);
            transform.localScale = finalScale;
        }
    }
}
