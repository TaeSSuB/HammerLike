using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class B_HPBar : MonoBehaviour
{
    [SerializeField] private Slider hpBarSlider;
    [SerializeField] private B_UnitBase unitBase;
    public void SetUnit(B_UnitBase b_UnitBase)
    {
        unitBase = b_UnitBase;
    }

    public void SetHPBar(float inCurrentHP, float inMaxHP)
    {
        hpBarSlider.value = inCurrentHP / inMaxHP;
    }

    protected void Update()
    {
        if (unitBase != null)
        {
            if(unitBase.UnitStatus.currentHP <= 0)
            {
                Destroy(gameObject);
                return;
            }

            SetHPBar(unitBase.UnitStatus.currentHP, unitBase.UnitStatus.maxHP);
        }
    }
}
