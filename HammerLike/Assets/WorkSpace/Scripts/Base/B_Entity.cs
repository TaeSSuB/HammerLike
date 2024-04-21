using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Entity : MonoBehaviour
{
    [Header("Base Object Info")]
    [SerializeField] protected GameObject rootObj;
    [SerializeField] protected GameObject meshObj;
    [SerializeField] protected Collider col;
    [SerializeField] protected Rigidbody rigid;

    [SerializeField] protected bool isInvincible;

    public GameObject MeshObj { get => meshObj; }

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

    protected virtual void OnCollisionEnter(Collision collision)
    {

    }

    public virtual void Init()
    {
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
