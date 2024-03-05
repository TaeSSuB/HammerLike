using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConnector : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        // 옵션 패널이 활성화될 때 UI 요소를 연결하는 로직
        SoundManager.Instance.ConnectUIElements();
    }
}
