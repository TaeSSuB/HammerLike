using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TradeTresure
{
    public Image tresureImage;
    public CTresure tresurePrefab;
    public int price;
    public bool isSold = false;
}

[System.Serializable]
public class TradeWeapon
{
    public Image weaponImage;
    public CWeapon weaponPrefab;
    public int price;
    public bool isSold = false;
}

public class Trade : MonoBehaviour
{
    public DataManager dm;
    public TextMeshProUGUI currentGoldText;
    public TradeTresure[] tradeTresures;
    public TradeWeapon[] tradeWeapons;
    //public Button[] itemButtons;

    public void tresureSell(int idx)
    {
        if (!tradeTresures[idx].isSold)
        {
            if (idx >= 0 && idx < tradeTresures.Length)
            {
                if (dm.currentGold >= tradeTresures[idx].price)
                {
                    dm.UseGold(tradeTresures[idx].price);
                    currentGoldText.text = dm.currentGold.ToString();

                    var player = GameObject.FindGameObjectWithTag("Player").GetComponent<CPlayer>();
                    tradeTresures[idx].tresurePrefab.PickUpTresureInstance(player);

                    tradeTresures[idx].isSold = true;
                }
                else
                {
                    Debug.LogWarning("Can not sell tresure");
                }
            }
        }
        else
            Debug.LogWarning("Is Already Sold");
    }

    public void weaponSell(int idx)
    {
        if (!tradeWeapons[idx].isSold)
        {
            if (idx >= 0 && idx < tradeWeapons.Length)
            {
                if (dm.currentGold >= tradeWeapons[idx].price)
                {
                    dm.UseGold(tradeWeapons[idx].price);
                    currentGoldText.text = dm.currentGold.ToString();

                    var player = GameObject.FindGameObjectWithTag("Player").GetComponent<CPlayer>();
                    tradeWeapons[idx].weaponPrefab.PickUpWeapon(player);

                    dm.saveWeapons.Add(tradeWeapons[idx].weaponPrefab);

                    tradeWeapons[idx].isSold = true;
                }
                else
                {
                    Debug.LogWarning("Can not sell weapon");
                }
            }
        }
        else
            Debug.LogWarning("Is Already Sold");
    }

    // Start is called before the first frame update
    void Start()
    {
        dm = DataManager.GetDataInstance();
        currentGoldText.text = dm.currentGold.ToString();
    }
}
