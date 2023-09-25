using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Explodable))]
public class GeneratePieceOnStart : MonoBehaviour
{
    Explodable explode;
    // Start is called before the first frame update
    private void OnEnable()
    {
        explode = GetComponent<Explodable>();
        //explode.OnStartGenerate();
    }

}
