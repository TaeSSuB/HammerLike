using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_UIManager : MonoBehaviour
{
    [SerializeField] private GameObject hpWorldUIPrefab;

    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryObj;
    [SerializeField] private KeyCode inventoryKey = KeyCode.I;

    // Start is called before the first frame update
    void Start()
    {
        
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
