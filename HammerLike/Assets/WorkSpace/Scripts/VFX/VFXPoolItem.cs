using UnityEngine;

public class VFXPoolItem : MonoBehaviour
{
    public void PlayVFX(float duration)
    {
        gameObject.SetActive(true);
        Invoke(nameof(ReturnToPool), duration);
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
