using UnityEngine;

[ExecuteInEditMode]
public class TransformVariate : MonoBehaviour
{
    public Vector3 minPosition;
    public Vector3 maxPosition;
    public Vector3 minRotation;
    public Vector3 maxRotation;
    public Vector3 minScale = Vector3.one;
    public Vector3 maxScale = Vector3.one;

    [Range(0, 1)] public float seed;

    private void Update()
    {
        Random.InitState(seed.GetHashCode());

        transform.localPosition = new Vector3(
            Random.Range(minPosition.x, maxPosition.x),
            Random.Range(minPosition.y, maxPosition.y),
            Random.Range(minPosition.z, maxPosition.z));

        transform.localRotation = Quaternion.Euler(
            Random.Range(minRotation.x, maxRotation.x),
            Random.Range(minRotation.y, maxRotation.y),
            Random.Range(minRotation.z, maxRotation.z));

        transform.localScale = new Vector3(
            Random.Range(minScale.x, maxScale.x),
            Random.Range(minScale.y, maxScale.y),
            Random.Range(minScale.z, maxScale.z));
    }
}