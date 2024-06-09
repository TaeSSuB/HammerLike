using UnityEngine;
using NuelLib;

public class B_UIManager : SingletonMonoBehaviour<B_UIManager>
{
    [SerializeField] private GameObject hpWorldUIPrefab;
    [SerializeField] private GameObject interactiveWorldUIPrefab;

    [Header("Ingame UI")]
    [SerializeField] private GameObject ingameUI;
    [SerializeField] private GameObject ingameUIPrefab;
    [SerializeField] private UI_InGame ui_InGame;

    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryObj;
    [SerializeField] private KeyCode inventoryKey = KeyCode.I;

    public UI_InGame UI_InGame { get => ui_InGame; }

    // IngameUI의 public getter 추가
    public GameObject IngameUI { get => ingameUI; }
    // Start is called before the first frame update
    void Start()
    {
        if(ingameUI == null) 
        {
            ingameUI = Instantiate(ingameUIPrefab);
            ingameUI.transform.SetParent(transform);
            ui_InGame = ingameUI.GetComponent<UI_InGame>();
        }
        else
        {
            ingameUI.SetActive(true);
            ui_InGame = ingameUI.GetComponent<UI_InGame>();
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(inventoryKey))
        {
            ToggleInventory();
        }
    }

    public void CreateHPWorldUI(Transform target, B_UnitBase unit)
    {
        GameObject hpWorldUI = Instantiate(hpWorldUIPrefab, target.position, Quaternion.identity);
        hpWorldUI.transform.SetParent(target);

        B_HPBar b_HPBar = hpWorldUI.GetComponent<B_HPBar>();
        
        if(b_HPBar == null) 
            b_HPBar  = hpWorldUI.AddComponent<B_HPBar>();
        
        b_HPBar.SetUnit(unit);
    }

    public GameObject CreateInteractiveWorldUI(Transform target)
    {
        GameObject interactiveWorldUI = Instantiate(interactiveWorldUIPrefab, target.position, Quaternion.identity);
        interactiveWorldUI.transform.SetParent(target);

        return interactiveWorldUI;
    }

    public void OpenInventory()
    {
        if (inventoryObj != null || inventoryObj.activeSelf)
            inventoryObj.SetActive(true);

        GameManager.Instance.SetTimeScale(0.1f);
    }

    public void CloseInventory()
    {
        GameManager.Instance.SetTimeScale(1f);
        
        if(inventoryObj != null && inventoryObj.activeSelf)
            inventoryObj.SetActive(false);
    }

    public void ToggleInventory()
    {
        if(inventoryObj.activeSelf)
            CloseInventory();
        else
            OpenInventory();
    }

    private void OnDisable()
    {
        CloseInventory();
    }
}
