using UnityEngine;

[ExecuteInEditMode]
public class SelectiveIndependentTransform : MonoBehaviour
{
    public bool independentPosition = false;
    public bool independentRotation = false;
    public bool independentScale = false;

    private Vector3 initialWorldPosition;
    private Quaternion initialWorldRotation;
    private Vector3 initialWorldScale;

    void Start()
    {
        SetInitialTransform();
    }

    void LateUpdate()
    {
        if (independentPosition)
            transform.position = initialWorldPosition;

        if (independentRotation)
            transform.rotation = initialWorldRotation;

        if (independentScale)
            ApplyWorldScale();
    }

    private void SetInitialTransform()
    {
        initialWorldPosition = transform.position;
        initialWorldRotation = transform.rotation;
        initialWorldScale = transform.lossyScale;
    }

    private void ApplyWorldScale()
    {
        if (transform.parent != null)
        {
            transform.localScale = new Vector3(
                independentScale ? initialWorldScale.x / transform.parent.lossyScale.x : transform.localScale.x,
                independentScale ? initialWorldScale.y / transform.parent.lossyScale.y : transform.localScale.y,
                independentScale ? initialWorldScale.z / transform.parent.lossyScale.z : transform.localScale.z
            );
        }
        else
        {
            if (independentScale)
                transform.localScale = initialWorldScale;
        }
    }

    void OnValidate()
    {
        SetInitialTransform();
    }
}
