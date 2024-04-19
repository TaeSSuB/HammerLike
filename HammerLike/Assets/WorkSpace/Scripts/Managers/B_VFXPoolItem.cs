using UnityEngine;

public class B_VFXPoolItem : MonoBehaviour
{
    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void PlayVFX()
    {
        gameObject.SetActive(true);
        particle.Play();
        Invoke(nameof(ReturnToPool), particle.main.duration);
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
