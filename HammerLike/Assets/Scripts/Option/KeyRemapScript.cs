using UnityEngine;
using UnityEngine.UI;
using Rewired;
using TMPro;
using System.Collections.Generic;
using Rewired.Data;

public class KeyRemapScript : MonoBehaviour
{
    [System.Serializable]
    public class KeyMap
    {
        public string actionName;
        public Pole axisContribution;
        public TMP_InputField inputField;
    }

    public List<KeyMap> keyMaps; // Inspector에서 설정할 수 있는 키 매핑 목록
    public Button changeKeyButton; // 키 변경 버튼
    private Rewired.Player player;
    private UserDataStore_PlayerPrefs userDataStore;

    void Start()
    {
        player = ReInput.players.GetPlayer(0);
        userDataStore = ReInput.userDataStore as UserDataStore_PlayerPrefs;

        changeKeyButton.onClick.AddListener(ChangeKeys);
    }

    public void ChangeKeys()
    {
        foreach (var keyMap in keyMaps)
        {
            KeyCode newKey;
            try
            {
                newKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyMap.inputField.text, true);
            }
            catch
            {
                Debug.LogError("Invalid KeyCode: " + keyMap.inputField.text);
                continue;
            }

            int actionId = ReInput.mapping.GetActionId(keyMap.actionName);
            foreach (ControllerMap map in player.controllers.maps.GetAllMaps(ControllerType.Keyboard))
            {
                foreach (ActionElementMap element in map.AllMaps)
                {
                    if (element.actionId == actionId && element.axisContribution == keyMap.axisContribution)
                    {
                        var elementIdentifier = ReInput.controllers.Keyboard.GetElementIdentifierByKeyCode(newKey);
                        if (elementIdentifier != null)
                        {
                            ElementAssignment assignment = new ElementAssignment(
                                ControllerType.Keyboard,
                                ControllerElementType.Button,
                                elementIdentifier.id,
                                AxisRange.Positive,
                                newKey,
                                ModifierKeyFlags.None,
                                element.actionId,
                                keyMap.axisContribution,
                                false,
                                element.id
                            );

                            map.ReplaceElementMap(assignment);
                        }
                    }
                }
            }
        }

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
