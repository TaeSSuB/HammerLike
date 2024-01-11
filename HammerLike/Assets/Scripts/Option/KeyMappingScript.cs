using UnityEngine;
using Rewired;
using UnityEngine.UI;
using System;


public class KeyMappingScript : MonoBehaviour
{
    public InputField inputField; // 입력 필드 참조
    private Rewired.Player player; // Rewired 플레이어

    void Start()
    {
        player = ReInput.players.GetPlayer(0); // 플레이어 설정
    }

    public void ApplyNewKey()
    {
        string newKey = inputField.text; // 입력 필드에서 새 키를 가져옵니다.

        // 키 매핑 업데이트
        foreach (var map in player.controllers.maps.GetAllMaps(ControllerType.Keyboard))
        {
            /*if (map.elementMaps.Find(x => x.actionName == "Horizontal") != null)
            {
                // 'Horizontal' 액션을 위한 새 키 설정
                map.elementMaps.Find(x => x.actionName == "Horizontal").elementIdentifierName = newKey;
            }*/
            
        }

        // 변경 사항 적용
        //player.controllers.maps.LoadAllMaps();
        //player.controllers.maps.();
    }
}
