using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateDungeon : MonoBehaviour
{
    public GameObject box;
    public GameObject dungeon;
    public int count = 4;
    public int radius = 4;
    public Vector3 sizeMin = new Vector3(1f, 1f, 1f);
    public Vector3 sizeMax = new Vector3(10f, 10f, 10f);
    
    private bool first = true;
    private List<GameObject> roomList = new List<GameObject>();
    // private List<GameObject> room = new List<GameObject>();

    Vector3 RandomPointInCircle()
    {
        var t = 2 * Mathf.PI * UnityEngine.Random.value;
        var u = UnityEngine.Random.value + UnityEngine.Random.value;
        float r;
        if (u > 1)
            r = 2 - u;
        else
            r = u;

        return new Vector3(
            Roundm((float)(radius * r * Math.Cos(t)), 4f),
            Roundm((float)(radius * r * Math.Sin(t)), 4f),
            1f
            ) ;
    }

    Vector3 RandomSize()
    {
        return new Vector3(
            UnityEngine.Random.Range(sizeMin.x, sizeMax.x),
            UnityEngine.Random.Range(sizeMin.y, sizeMax.y),
            UnityEngine.Random.Range(sizeMin.z, sizeMax.z)
            );
    }

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < count; i++)
        {
            GameObject go = Instantiate<GameObject>(box);
            if (go != null)
            {
                roomList.Add(go);
                go.transform.position = RandomPointInCircle();
                go.transform.localScale = RandomSize();
            }
        }
    }

    float Roundm(float n, float m)
    {
        return (float)Math.Floor(((n + m - 1) / m) * m);
    }

    void Selection()
    {
        foreach (var go in roomList)
        {
            var x = go.transform.localScale.x;
            var y = go.transform.localScale.y;

            if (x <= 30.0f || y <= 30.0f)
            {
                go.SetActive(false);
            }
        }

        first = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (first)
            {
            foreach (var go in roomList)
            {
                var rigidbody = go.GetComponent<Rigidbody2D>();
                if (!rigidbody.IsSleeping())
                {
                    return;
                }
            }

            Selection();
        }
    }
}
