using UnityEngine;
using UnityEngine.UI;

public class KeyBinder : MonoBehaviour
{
    public string action;

    public void OnClick()
    {
        FindObjectOfType<KeyMappingManager>().StartKeyBinding(action);
    }
}
