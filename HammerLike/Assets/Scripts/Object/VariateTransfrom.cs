using UnityEngine;

public class VariateTransform : MonoBehaviour
{
    [SerializeField] private Vector3 positionVariance;
    [SerializeField] private Vector3 rotationVariance;
    [SerializeField] private Vector3 scaleVariance;

    private void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    private void Update()
    {
        transform.position += new Vector3(
            Random.Range(-positionVariance.x, positionVariance.x),
            Random.Range(-positionVariance.y, positionVariance.y),
            Random.Range(-positionVariance.z, positionVariance.z));

        transform.eulerAngles += new Vector3(
            Random.Range(-rotationVariance.x, rotationVariance.x),
            Random.Range(-rotationVariance.y, rotationVariance.y),
            Random.Range(-rotationVariance.z, rotationVariance.z));

        transform.localScale += new Vector3(
            Random.Range(-scaleVariance.x, scaleVariance.x),
            Random.Range(-scaleVariance.y, scaleVariance.y),
            Random.Range(-scaleVariance.z, scaleVariance.z));
    }
}