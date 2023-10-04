using UnityEngine;
using TMPro;

public class Test : MonoBehaviour
{
    public TMP_Text tmpText;

    private void Awake()
    {
        tmpText.GetComponent<RectTransform>().sizeDelta = new Vector2(tmpText.preferredWidth, tmpText.preferredHeight);
    }
}