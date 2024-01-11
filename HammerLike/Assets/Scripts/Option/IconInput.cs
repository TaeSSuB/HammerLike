using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Rewired;

[System.Serializable]
public class AlphabetIconMapping
{
    public char alphabet; // 문자
    public Sprite icon; // 아이콘
}

public class IconInput : MonoBehaviour
{
    public TMP_InputField inputField;
    public Image iconDisplay; // 아이콘을 표시할 Image 컴포넌트
    public List<AlphabetIconMapping> alphabetIconsMappings; // 문자와 아이콘 매핑 목록
    public string actionName; // 액션 이름
    public Pole axisContribution = Pole.Positive; // Positive 또는 Negative

    private Dictionary<char, Sprite> alphabetIcons = new Dictionary<char, Sprite>();
    private Rewired.Player player; // Rewired Player

    void Start()
    {
        player = ReInput.players.GetPlayer(0);

        foreach (var mapping in alphabetIconsMappings)
        {
            if (!alphabetIcons.ContainsKey(mapping.alphabet))
            {
                alphabetIcons[mapping.alphabet] = mapping.icon;
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
            char lastChar = input[input.Length - 1].ToString().ToUpper()[0];
            if (alphabetIcons.ContainsKey(lastChar))
            {
                iconDisplay.sprite = alphabetIcons[lastChar];
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
                    char keyChar = currentKey.ToString().ToUpper()[0];
                    if (alphabetIcons.ContainsKey(keyChar))
                    {
                        iconDisplay.sprite = alphabetIcons[keyChar];
                        return;
                    }
                }
            }
        }
        iconDisplay.sprite = null;
    }
}
