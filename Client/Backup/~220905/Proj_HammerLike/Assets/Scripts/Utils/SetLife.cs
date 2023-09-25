using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLife : MonoBehaviour
{
    public float lifeTime = 5f;
    //public SetLife(GameObject _gameObject, float _time)
    //{

    //}

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
