using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_UIManager : MonoBehaviour
{
    [SerializeField] private GameObject hpWorldUIPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateHPWorldUI(Transform target, B_UnitBase unit)
    {
        GameObject hpWorldUI = Instantiate(hpWorldUIPrefab, target.position, Quaternion.identity);
        hpWorldUI.transform.SetParent(target);

        B_HPBar b_HPBar = hpWorldUI.GetComponent<B_HPBar>();
        b_HPBar.SetUnit(unit);
    }
}
