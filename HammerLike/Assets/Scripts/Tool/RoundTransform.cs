using UnityEngine;

public class RoundTransform : MonoBehaviour
{
    public void RoundTransformValues()
    {
        Transform t = this.gameObject.transform;

        t.position = RoundVector3ToHalf(t.position);
        t.eulerAngles = RoundVector3ToHalf(t.eulerAngles);
        t.localScale = RoundVector3ToHalf(t.localScale);
    }

    private Vector3 RoundVector3ToHalf(Vector3 vector)
    {
        return new Vector3(
            Mathf.Round(vector.x * 2) / 2,
            Mathf.Round(vector.y * 2) / 2,
            Mathf.Round(vector.z * 2) / 2);
    }
}