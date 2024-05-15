
using UnityEngine;

/// <summary>
/// B_Entity : Entity Base Class
/// - Entity의 기본적인 정보를 담고 있는 클래스
/// - 초기화 및 기본 기능들을 상속 받아 사용
/// </summary>
public class B_Entity : MonoBehaviour
{
    [Header("Base Object Info")]
    [SerializeField] protected GameObject rootObj;
    [SerializeField] protected GameObject meshObj;
    [SerializeField] protected Collider col;
    [SerializeField] private Rigidbody rigid;

    [SerializeField] protected bool isInvincible;

    public GameObject RootObj { get => rootObj; }
    public GameObject MeshObj { get => meshObj; }
    public Collider Col { get => col; }
    public Rigidbody Rigid { get => rigid;}

    #region Unity Callbacks & Init
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

    protected virtual void FixedUpdate()
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
    #endregion

    #region Check or Update State
    
    /// <summary>
    /// SetInvincible : 무적 상태 설정
    /// </summary>
    /// <param name="inIsInvincible"></param>
    public void SetInvincible(bool inIsInvincible)
    {
        isInvincible = inIsInvincible;
    }

    #endregion
}
