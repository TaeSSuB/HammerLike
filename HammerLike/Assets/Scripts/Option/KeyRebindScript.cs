using UnityEngine;
using UnityEngine.UI;
using Rewired;
using TMPro; // TextMesh Pro 네임스페이스 추가
using Rewired.Data;

public class KeyRebindScript : MonoBehaviour
{
    public TMP_InputField inputField; // TMP 입력 필드
    public Button changeKeyButton; // 키 변경 버튼
    private Rewired.Player player; // Rewired 플레이어
    private UserDataStore_PlayerPrefs userDataStore; // 사용자 데이터 저장소

    void Start()
    {
        player = ReInput.players.GetPlayer(0); // 첫 번째 플레이어 가져오기
        userDataStore = ReInput.userDataStore as UserDataStore_PlayerPrefs; // UserDataStore_PlayerPrefs 인스턴스 가져오기

        // 버튼 클릭 이벤트에 ChangeKey 메서드 추가
        changeKeyButton.onClick.AddListener(() => ChangeKey(inputField.text));
    }

    public void ChangeKey(string newKey)
    {
        KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), newKey, true); // 문자열을 KeyCode로 변환

        int categoryId = 1; // 예를 들어, 카테고리 ID가 1번이라고 가정
        foreach (ControllerMap map in player.controllers.maps.GetAllMaps(ControllerType.Keyboard))
        {
            if (map.categoryId != categoryId)
            {
                Debug.Log("잘못된 카테고리");
                continue; // 올바른 카테고리인지 확인
            }

                foreach (ActionElementMap element in map.AllMaps)
            {
                if (element.actionDescriptiveName == "Horizontal" && element.axisContribution == Pole.Positive) // Horizontal의 Positive Key를 찾기
                {
                    var elementIdentifier = ReInput.controllers.Keyboard.GetElementIdentifierByKeyCode(keyCode);
                    if (elementIdentifier != null)
                    {
                        ElementAssignment assignment = new ElementAssignment(
                            ControllerType.Keyboard,
                            ControllerElementType.Button,
                            elementIdentifier.id,
                            AxisRange.Positive,
                            keyCode,
                            ModifierKeyFlags.None,
                            element.actionId,
                            Pole.Positive,
                            false,
                            element.id
                        );

                        // 키 변경
                        map.ReplaceElementMap(assignment);
                    }
                    break;
                }
            }
        }

        // 변경 사항 저장
        if (userDataStore != null)
        {
            userDataStore.Save();
        }
        else
        {
            Debug.LogError("UserDataStore_PlayerPrefs not found!");
        }
    }
}
