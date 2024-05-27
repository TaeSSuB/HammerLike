using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    public Material baseMat;

    public float scrollX = 1f;
    public float scrollY = 1f;

    // Start is called before the first frame update
    private void Start()
    {
        if (baseMat == null) baseMat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    private void Update()
    {
        float offsetX = Time.time * scrollX;
        float offsetY = Time.time * scrollY;

        baseMat.mainTextureOffset = new Vector2 (offsetX, offsetY);
    }
}
