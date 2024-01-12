using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Rewired;

[System.Serializable]
public class AlphabetIconMapping
{
    public string keyName; // 키 이름
    public Sprite icon; // 아이콘
}

public class IconInput : MonoBehaviour
{
    public TMP_InputField inputField;
    public Image iconDisplay;
    public List<AlphabetIconMapping> keyIconsMappings;
    public string actionName;
    public Pole axisContribution = Pole.Positive;

    private Dictionary<string, Sprite> keyIcons = new Dictionary<string, Sprite>();
    private Rewired.Player player;

    void Start()
    {
        player = ReInput.players.GetPlayer(0);

        foreach (var mapping in keyIconsMappings)
        {
            string keyNameUpper = mapping.keyName.ToUpper(); // 키 이름을 대문자로 변환
            if (!keyIcons.ContainsKey(keyNameUpper))
            {
                keyIcons[keyNameUpper] = mapping.icon;
            }
        }

        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(HandleInputChanged);
            SetInitialIcon();
        }
    }

    private void HandleInputChanged(string input)
    {
        if (input.Length > 0)
        {
            string inputUpper = input.ToUpper(); // 입력을 대문자로 변환
            if (keyIcons.ContainsKey(inputUpper))
            {
                iconDisplay.sprite = keyIcons[inputUpper];
            }
        }
    }

    private void SetInitialIcon()
    {
        int actionId = ReInput.mapping.GetActionId(actionName);
        foreach (ControllerMap map in player.controllers.maps.GetAllMaps(ControllerType.Keyboard))
        {
            foreach (ActionElementMap element in map.AllMaps)
            {
                if (element.actionId == actionId && element.axisContribution == axisContribution)
                {
                    KeyCode currentKey = ReInput.controllers.Keyboard.GetKeyCodeById(element.elementIdentifierId);
                    string keyNameUpper = currentKey.ToString().ToUpper(); // 키 이름을 대문자로 변환
                    if (keyIcons.ContainsKey(keyNameUpper))
                    {
                        iconDisplay.sprite = keyIcons[keyNameUpper];
                        return;
                    }
                }
            }
        }
        iconDisplay.sprite = null;
    }
}
