using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_ObjectBase : MonoBehaviour
{
    [Header("Manager (Temp - 240331), a.HG")]
    [SerializeField] protected GameManager manager;

    [Header("Base Object Info")]
    [SerializeField] protected GameObject rootObj;
    [SerializeField] protected Collider col;
    [SerializeField] protected Rigidbody rigid;

    [SerializeField] protected bool isInvincible;

    protected virtual void Awake()
    {
        //Init();
    }
    protected virtual void OnEnable()
    {
        //Init();
    }

    protected virtual void Start()
    {
        Init();
    }

    protected virtual void Update()
    {

    }

    protected virtual void OnTriggerEnter(Collider other)
    {

    }

    public virtual void Init()
    {
        if(manager == null)
        {
            manager = GameManager.instance;
        }

        if(rootObj == null)
        {
            rootObj = gameObject;
        }

        if(col == null)
        {
            col = GetComponent<Collider>();

            if (col == null)
            {
                col = gameObject.AddComponent<SphereCollider>();
                Debug.Log("bind none. Add new one.");
            }
        }

        if(rigid == null)
        {
            rigid = GetComponent<Rigidbody>();

            if (rigid == null)
            {
                rigid = gameObject.AddComponent<Rigidbody>();
                Debug.Log("bind none. Add new one.");
            }
        }
    }

    public void SetInvincible(bool inIsInvincible)
    {
        isInvincible = inIsInvincible;
    }
}
