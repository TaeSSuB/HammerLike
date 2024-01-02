using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeyMappingManager : MonoBehaviour
{
    public Text jumpKeyText;

    private void Update()
    {
        if (InputManager.instance.GetKeyDown("JumpKey"))
        {
            Debug.Log("Jump!");
        }

        // Update UI text with current key mapping
        jumpKeyText.text = "Jump Key: " + InputManager.instance.GetKeyMapping("JumpKey");
    }

    public void StartKeyBinding(string action)
    {
        StartCoroutine(BindKey(action));
    }

    private IEnumerator BindKey(string action)
    {
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                InputManager.instance.SetKeyMapping(action, keyCode);
                break;
            }
        }
    }
}
