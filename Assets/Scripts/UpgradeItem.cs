using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BayatGames.SaveGameFree;

public class UpgradeItem : MonoBehaviour
{

    [Header ("Type of upgrade")]
    public bool isSpecificItemSpeed;
    public bool isSpecificItemProfit;
    public bool isGeneralProfit;
    public bool isOG;

    [Header ("Settings")]
    public int itemInList;
    public float upgradeFactor;
    public float upgradecost;

    [Header("References")]
    public bool isBought;
    public Button button;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI infoText;
    public Inventory2Script inventory;

    //private static List<string> nFormat = new List<string>();

    private void Start()
    {
        if (SaveGame.Exists("Bought" + gameObject.GetInstanceID()))
        {
            isBought = SaveGame.Load<bool>("Bought" + gameObject.GetInstanceID());
        }

        inventory.upgradeObjects.Add(gameObject);
        checkIfBought();

        inventory = GameObject.FindGameObjectWithTag("inventory2").GetComponent<Inventory2Script>();

        //buttonText.text = upgradecost.ToString();
        buttonText.text = FormatNumber(upgradecost);
        
        if (isSpecificItemSpeed)
        {
            infoText.text = inventory.totalItems[itemInList].name + " speed x " + (upgradeFactor + 1);
        }
        else if (isSpecificItemProfit)
        {
            infoText.text = inventory.totalItems[itemInList].name + " profit x " + (upgradeFactor + 1);
        }
        else if (isGeneralProfit)
        {
            infoText.text = "All business profit x " + (upgradeFactor + 1);
        }
        else if (isOG)
        {
            infoText.text = "Profit per OG + " + upgradeFactor + "%";
        }

        button.onClick.AddListener(Listener);
    }

    public void checkIfBought()
    {
        if(isBought)
        {
            runAdvantage();
        }
    }

    public void runAdvantage()
    {
        if (isSpecificItemProfit)
        {
            inventory.totalItems[itemInList].revenueMultiplier += upgradeFactor;
        }
        if (isSpecificItemSpeed)
        {
            inventory.totalItems[itemInList].timeMultiplier += upgradeFactor;
        }
        if (isGeneralProfit)
        {
            inventory.multiplierUpgradeTree += upgradeFactor;
        }
        if (isOG)
        {
            inventory.respawnMultiplier += upgradeFactor;
        }

        isBought = true;
        SaveGame.Save<bool>("Bought" + gameObject.GetInstanceID(), isBought);
        inventory.RespawnCalculater();
        gameObject.SetActive(false);
    }

    public void Listener()
    {
        if (inventory.currentRespawns >= upgradecost && !isBought)
        {
            inventory.currentRespawns -= upgradecost;
            runAdvantage();
        }
    }

    public static string FormatNumber(double value)
    {
        if (value < 1d)
        {
            return "0";
        }

        var n = (int)Math.Log(value, 1000);
        var m = value / Math.Pow(1000, n);
        var unit = "";

        if (n < Inventory2Script.Numberunits.Count)
        {
            unit = Inventory2Script.Numberunits[n];
        }
        else
        {
            var unitInt = n - Inventory2Script.Numberunits.Count;
            var secondUnit = unitInt % 26;
            var firstUnit = unitInt / 26;
            unit = Convert.ToChar(firstUnit + Inventory2Script.charA).ToString() + Convert.ToChar(secondUnit + Inventory2Script.charA).ToString();
        }

        // Math.Floor(m * 100) / 100) fixes rounding errors
        return (Math.Floor(m * 100) / 100).ToString("0.##") + unit;
    }
}
