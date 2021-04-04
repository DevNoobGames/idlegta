using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class inventoryScript : MonoBehaviour
{
    public float money;
    public TextMeshProUGUI moneyText;

    public float lifetimeMoney;
    public int currentRespawns;
    public int newRespawns;
    public TextMeshProUGUI respawnText;

    public int activeBuyInt = 1;
    public TextMeshProUGUI activeBuyText;

    public GameObject panel;

    public float multiplier;

    public items[] totalItems;
    [System.Serializable]
    public class items
    {
        public string name;
        public Sprite iconspr;
        public float owned;
        public bool automated;
        public float initialCost;
        public float costNext;
        public float CanBuy;
        public float timeMultiplier;
        public float revenueMultiplier;
        public float coefficient;
        public float initialTime;
        public float initialRevenue;
        public float initialProductivity;
        public float multiplier25;
        public float multiplier50;
        public float multiplier100;
        public float multiplier1000;
        public float multiplier10000;
        public float multiplier100000;
        public float multiplier10000000;


        [HideInInspector] public bool isRunning;
        [HideInInspector] public Slider slider;
        [HideInInspector] public Button Buybutton;
        [HideInInspector] public Button runOncebutton;

        [HideInInspector] public TextMeshProUGUI BuybuttonText;
        [HideInInspector] public TextMeshProUGUI runOnceButtonText;
        //HideInInspector] public TextMeshProUGUI amountText;
        [HideInInspector] public TextMeshProUGUI moneyPerSecondText;
    }

    

    void Start()
    {
        //TESTING
        lifetimeMoney = 500000;
        //

        moneyText.text = money.ToString("F2") + "$";

        foreach (items i in totalItems)
        {
            GameObject item = Instantiate(Resources.Load("Item"), transform.position, Quaternion.identity) as GameObject;
            item.transform.SetParent(panel.transform, false);
            itemInfo other = item.GetComponent<itemInfo>();
            
            i.slider = other.slider;
            i.slider.value = 0;
            i.slider.maxValue = i.initialTime / i.timeMultiplier;

            i.Buybutton = other.Buybutton;
            i.Buybutton.onClick.AddListener(delegate { AddListenerInfo(i); });
            i.BuybuttonText = other.BuybuttonText;

            i.runOncebutton = other.runOnceButton;
            i.runOncebutton.onClick.AddListener(delegate { AddListenerRunOne(i); });
            i.runOnceButtonText = other.runOnceButtonText;
            i.runOnceButtonText.text = i.name + "\n" + i.owned;

            i.moneyPerSecondText = other.moneyPerSecondText;
            setiMoneyText(i);

            if (i.automated)
            {
                StartCoroutine(RevenueCoroutine(i));
            }
        }
        nextCostCalculator();
    }

    public void setiMoneyText(items i)
    {
        if (i.initialTime / i.timeMultiplier > 0.2f)
        {
            i.moneyPerSecondText.text = (i.initialRevenue * i.owned * multiplier * i.revenueMultiplier * i.multiplier25 * i.multiplier50 * i.multiplier100 * i.multiplier1000 * i.multiplier10000 * i.multiplier100000 * i.multiplier10000000).ToString("F2");
        }
        else
        {
            i.moneyPerSecondText.text = ((i.initialRevenue * i.owned * multiplier * i.revenueMultiplier * i.multiplier25 * i.multiplier50 * i.multiplier100 * i.multiplier1000 * i.multiplier10000 * i.multiplier100000 * i.multiplier10000000) / (i.initialTime / i.timeMultiplier)).ToString("F2") + "/s";
        }
    }

    public void bulkBuyButton()
    {
        if (activeBuyInt == 1)
        {
            activeBuyInt = 10;
            activeBuyText.text = activeBuyInt.ToString();
        }
        else if (activeBuyInt == 10)
        {
            activeBuyInt = 100;
            activeBuyText.text = activeBuyInt.ToString();
        }
        else if (activeBuyInt == 100)
        {
            activeBuyInt = 1000;
            activeBuyText.text = activeBuyInt.ToString();
        }
        else if (activeBuyInt == 1000)
        {
            activeBuyInt = 99999;
            activeBuyText.text = "MAX";
        }
        else if (activeBuyInt == 99999)
        {
            activeBuyInt = 1;
            activeBuyText.text = activeBuyInt.ToString();
        }
        nextCostCalculator();

    }

    IEnumerator RevenueCoroutine(items i)
    {
        i.isRunning = true;
        if (i.initialTime / i.timeMultiplier > 0.2f)
        {
            i.slider.value = 0;
            StartCoroutine(sliderCoroutine(i));
        }
        else
        {
            i.slider.value = i.slider.maxValue;
        }
        setiMoneyText(i);

        yield return new WaitForSeconds(i.initialTime / i.timeMultiplier);
        money += i.initialRevenue * i.owned * multiplier * i.revenueMultiplier * i.multiplier25 * i.multiplier50 * i.multiplier100 * i.multiplier1000 * i.multiplier10000 * i.multiplier100000 * i.multiplier10000000;
        lifetimeMoney += i.initialRevenue * i.owned * multiplier * i.revenueMultiplier * i.multiplier25 * i.multiplier50 * i.multiplier100 * i.multiplier1000 * i.multiplier10000 * i.multiplier100000 * i.multiplier10000000;
        moneyText.text = money.ToString("F2") + "$";
        RespawnCalculater();
        i.isRunning = false;
        i.slider.value = 0;
        if (activeBuyInt == 99999)
        {
            nextCostCalculator();
        }
        if (i.automated)
        {
            StartCoroutine(RevenueCoroutine(i));
        }
    }

    IEnumerator sliderCoroutine(items i)
    {
        i.slider.maxValue = i.initialTime / i.timeMultiplier;

        float animationTime = 0f;
        while (animationTime < (i.initialTime / i.timeMultiplier))
        {
            animationTime += Time.deltaTime;
            float lerpValue = animationTime / (i.initialTime / i.timeMultiplier);
            i.slider.value = Mathf.Lerp(0f, (i.initialTime / i.timeMultiplier), lerpValue);
            yield return null;
        }
    }

    public void nextCostCalculator()
    {
        foreach (items i in totalItems)
        {
            if (activeBuyInt == 1)
            {
                float nextCost = i.initialCost * Mathf.Pow(i.coefficient, i.owned);
                i.CanBuy = 1;
                i.costNext = Mathf.Floor(nextCost);
                i.BuybuttonText.text = "Buy 1" + "\n" + i.costNext + "$";

            }
            if (activeBuyInt > 1 && activeBuyInt < 99998)
            {
                float nextCost = i.initialCost * ((Mathf.Pow(i.coefficient, i.owned) - Mathf.Pow(i.coefficient, (i.owned + activeBuyInt))) / (1 - i.coefficient));
                i.CanBuy = activeBuyInt;
                i.costNext = Mathf.Floor(nextCost);
                i.BuybuttonText.text = "Buy " + i.CanBuy + "\n" + i.costNext + "$";
            }
            if (activeBuyInt == 99999)
            {
                float maxBuy = Mathf.Floor(Mathf.Log(Mathf.Pow(i.coefficient, i.owned) - (money * (1 - i.coefficient) / i.initialCost), i.coefficient) - i.owned);
                i.CanBuy = maxBuy;
                float nextCost = i.initialCost * ((Mathf.Pow(i.coefficient, i.owned) - Mathf.Pow(i.coefficient, (i.owned + i.CanBuy))) / (1 - i.coefficient));
                i.costNext = Mathf.Floor(nextCost);
                i.BuybuttonText.text = "Buy " + i.CanBuy + "\n" + i.costNext + "$";
            }
        }
    }

    public void RespawnCalculater()
    {
        newRespawns = Mathf.FloorToInt(lifetimeMoney / 10000) - currentRespawns;
        respawnText.text ="Respawn with " + newRespawns.ToString() + "% bonus";
    }

    public void Respawn()
    {
        if (newRespawns > 1)
        {
            currentRespawns += newRespawns;
            multiplier = 1f + (currentRespawns / 100f);
            resetStats();
        }
        RespawnCalculater();
    }

    public void resetStats()
    {
        money = 10;
        moneyText.text = money.ToString("F2");
        StopAllCoroutines();
        foreach (items i in totalItems)
        {
            //reset values
            i.slider.value = 0;
            i.owned = 0;
            i.automated = false;
            i.timeMultiplier = 1;
            i.revenueMultiplier = 1;
            i.isRunning = false;

            //reset text
            i.runOnceButtonText.text = i.name + "\n" + i.owned;
            setiMoneyText(i);

        }

        //give first for free
        totalItems[0].owned = 1;
        totalItems[0].runOnceButtonText.text = totalItems[0].name + "\n" + totalItems[0].owned;
        setiMoneyText(totalItems[0]);



        nextCostCalculator();
    }

    public void AddListenerRunOne(items i)
    {
        if (!i.automated && !i.isRunning && i.owned > 0)
        {
            i.isRunning = true;
            StartCoroutine(RevenueCoroutine(i));
        }
    }

    public void AddListenerInfo(items i)
    {
        if (i.costNext < money)
        {
            money -= i.costNext;
            moneyText.text = money.ToString("F2") + "$";

            i.owned += i.CanBuy;
            i.runOnceButtonText.text = i.name + "\n" + i.owned;

            setiMoneyText(i);

            nextCostCalculator();
        }
    }

}





/*float firstPow = Mathf.Pow(i.coefficient, i.owned);
float firstN = Mathf.Log(firstPow - (money * (1 - i.coefficient) / i.initialCost), i.coefficient) - i.owned;
float secondN = Mathf.Floor(firstN);*/
