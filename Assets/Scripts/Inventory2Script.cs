using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BayatGames.SaveGameFree;

public class Inventory2Script : MonoBehaviour
{
    public float money;
    public TextMeshProUGUI moneyText;
    public float moneyperS1;
    public float moneyperS2;
    public TextMeshProUGUI moneyPerSText;

    public float lifetimeMoney;
    public float currentRespawns;
    public float newRespawns;
    public float TotalRespawnsBought;
    public TextMeshProUGUI currentOGText;
    public TextMeshProUGUI respawnText;

    public int activeBuyInt = 1;
    public TextMeshProUGUI activeBuyText;

    public int Version;
    public GameObject canvas;
    public Menus menu;
    public GameObject managerContainer;
    public TextMeshProUGUI upgradeOGText;

    public float respawnMultiplier;
    public float multiplier;
    public float multiplierUpgradeTree;

    public List<GameObject> upgradeObjects;
    public List<GameObject> managersObjects;

    public GameObject welcomeScreen;
    public TextMeshProUGUI welcomeStats;

    public AudioSource achievementSound;
    public AudioSource coinSound;
    public AudioSource BGSound;

    public GameObject tut1;
    public GameObject tut2;
    public GameObject tut3;
    public GameObject tut4;
    public GameObject tut5;
    public GameObject tut6;
    public int activeTutorial;

    DateTime currentDate;
    DateTime oldDate;

    //Test with number sizes
    public static readonly int charA = Convert.ToInt32('a');

    public float timerToPause;

    public items[] totalItems;
    [System.Serializable]
    public class items
    {
        public string name;
        public GameObject item;
        public float owned; //needs to be saved
        public bool automated; //loaded with managers
        public GameObject spawnPos;
        public float initialCost;
        public float costNext;
        public float CanBuy;
        public float timeMultiplier; //loaded with managers and upgrades
        public float revenueMultiplier; //loaded with managers and upgrades
        public float coefficient;
        public float initialTime;
        public float initialRevenue;
        public float initialProductivity;
        public float multiplierTree; //loaded with managers and upgrades
        public float multiplier25; //for all, calculated later
        public float multiplier50;
        public float multiplier100;
        public float multiplier1000;
        public float multiplier10000;
        public float multiplier100000;
        public float multiplier10000000;
        public int hasHit25;
        public int hasHit50;
        public int hasHit100;
        public int hasHit1000;
        public int hasHit10000;
        public int hasHit100000;
        public int hasHit10000000;

        [Header ("Manager")]
        public Sprite managerSpr;
        public string managerName;
        public float managerCost;
        public bool managerIsBought;

        [HideInInspector] public bool isRunning;
        [HideInInspector] public Slider slider;
        [HideInInspector] public Slider slider2;
        [HideInInspector] public Image radialImage;
        [HideInInspector] public Button Buybutton;
        [HideInInspector] public Button runOncebutton;

        [HideInInspector] public TextMeshProUGUI BuybuttonText;
        [HideInInspector] public TextMeshProUGUI runOnceButtonText;
        [HideInInspector] public TextMeshProUGUI nameText;
        [HideInInspector] public TextMeshProUGUI moneyPerSecondText;
    }


    void Start()
    {

        //PlayerPrefs.DeleteAll();
        //SaveGame.DeleteAll();

        activeTutorial = 0;
        /*if (SaveGame.Exists("Money"))
        {
            money = SaveGame.Load<float>("Money");
        }
        if (SaveGame.Exists("lifetimeMoney"))
        {
            lifetimeMoney = SaveGame.Load<float>("lifetimeMoney");
        }
        if (SaveGame.Exists("currentRespawns"))
        {
            currentRespawns = SaveGame.Load<float>("currentRespawns");
        }
        if (SaveGame.Exists("TotalRespawnsBought"))
        {
            TotalRespawnsBought = SaveGame.Load<float>("TotalRespawnsBought");
        }*/

        money = PlayerPrefs.GetFloat("Money");
        if (money == 0)
        {
            money = 20;
        }
        lifetimeMoney = PlayerPrefs.GetFloat("lifetimeMoney");
        currentRespawns = PlayerPrefs.GetFloat("currentRespawns");
        TotalRespawnsBought = PlayerPrefs.GetFloat("TotalRespawnsBought");



        //respawn multiplier is already loaded in upgrades bought
        //Same with multiplier
        //same with multiplierupgradetree

        foreach (items i in totalItems)
        {

            itemInfo2 other = i.item.GetComponent<itemInfo2>();


            if (Version == 2)
            {
                i.radialImage = other.radialImage;
                i.radialImage.fillAmount = 0;
            }

            i.Buybutton = other.Buybutton;
            i.Buybutton.onClick.AddListener(delegate { AddListenerInfo(i); });
            i.BuybuttonText = other.BuybuttonText;

            i.runOncebutton = other.runOnceButton;
            i.runOncebutton.onClick.AddListener(delegate { AddListenerRunOne(i); });

            i.moneyPerSecondText = other.moneyPerSecondText;

            i.hasHit25 = PlayerPrefs.GetInt(i.name + ".hasHit25");
            i.hasHit50 = PlayerPrefs.GetInt(i.name + ".hasHit50");
            i.hasHit100 = PlayerPrefs.GetInt(i.name + ".hasHit100");
            i.hasHit1000 = PlayerPrefs.GetInt(i.name + ".hasHit1000");
            i.hasHit10000 = PlayerPrefs.GetInt(i.name + ".hasHit10000");
            i.hasHit100000 = PlayerPrefs.GetInt(i.name + ".hasHit100000");
            i.hasHit10000000 = PlayerPrefs.GetInt(i.name + ".hasHit10000000");

            if (SaveGame.Exists("owned" + i.name))
            {
                i.owned = SaveGame.Load<float>("owned" + i.name);
            }

            setiMoneyText(i);

            if (i.automated && i.owned > 0)
            {
                StartCoroutine(RevenueCoroutine(i));
            }
        }

        SetMoneyText();
        nextCostCalculator();
        setManagers();
        RespawnCalculater();
        moneyPerScalc();

        if (PlayerPrefs.HasKey("sysString"))
        {
            currentDate = System.DateTime.Now;
            long temp = Convert.ToInt64(PlayerPrefs.GetString("sysString"));
            DateTime oldDate = DateTime.FromBinary(temp);
            TimeSpan difference = currentDate.Subtract(oldDate);
            welcomeScreen.SetActive(true);
            if (difference.Seconds > 0)
            {
                if (difference.Hours < 4)
                {
                    float moneyDiff = moneyperS2 * difference.Seconds;
                    welcomeStats.text = "You been away for: " + difference.Hours + " hours " + difference.Minutes + " minutes " + difference.Seconds + " seconds" + "\n" + "\n" + "You made " + FormatNumber(moneyDiff);
                    money += moneyDiff;
                }
                else
                {
                    float moneyDiff = moneyperS2 * 60 * 60 * 4; //60 seconds, 60 minutes, 4 hours
                    welcomeStats.text = "You been away for more than 4 hours" + "\n" + "\n" + "You made " + FormatNumber(moneyDiff) + "$";
                    money += moneyDiff;
                }
            }
        }
        else
        {
            menu.TutorialActive = true;
            tut1.SetActive(true);
            activeTutorial = 1;
            //start tutorial
        }
    }

    private void Update()
    {
        timerToPause += Time.deltaTime;
    }

    public void closeWelcomeMenu()
    {
        welcomeScreen.SetActive(false);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && timerToPause > 2)
        {
            PlayerPrefs.SetFloat("Money", money);
            PlayerPrefs.SetFloat("lifetimeMoney", lifetimeMoney);
            PlayerPrefs.SetFloat("currentRespawns", currentRespawns);
            PlayerPrefs.SetFloat("TotalRespawnsBought", TotalRespawnsBought);

            PlayerPrefs.SetString("sysString", System.DateTime.Now.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }

    /*private void OnDestroy()
    {
        PlayerPrefs.SetFloat("Money", money);
        PlayerPrefs.SetFloat("lifetimeMoney", lifetimeMoney);
        PlayerPrefs.SetFloat("currentRespawns", currentRespawns);
        PlayerPrefs.SetFloat("TotalRespawnsBought", TotalRespawnsBought);

        PlayerPrefs.SetString("sysString", System.DateTime.Now.ToBinary().ToString());
        PlayerPrefs.Save();
    }*/

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("Money", money);
        PlayerPrefs.SetFloat("lifetimeMoney", lifetimeMoney);
        PlayerPrefs.SetFloat("currentRespawns", currentRespawns);
        PlayerPrefs.SetFloat("TotalRespawnsBought", TotalRespawnsBought);

        PlayerPrefs.SetString("sysString", System.DateTime.Now.ToBinary().ToString());
        PlayerPrefs.Save();

        //SaveGame.Save<float>("Money", money);
        /*SaveGame.Save<float>("lifetimeMoney", lifetimeMoney);
        SaveGame.Save<float>("currentRespawns", currentRespawns);
        SaveGame.Save<float>("TotalRespawnsBought", TotalRespawnsBought);*/
    }

    public void setManagers()
    {
        if (managersObjects.Count > 0)
        {
            foreach (GameObject man in managersObjects)
            {
                Destroy(man);
            }
        }

        managersObjects.Clear();

        foreach (items i in totalItems)
        {
            GameObject managerItem = Instantiate(Resources.Load("Manager"), transform.position, Quaternion.identity) as GameObject;
            managerItem.transform.SetParent(managerContainer.transform, false);
            ManagerMenu other = managerItem.GetComponent<ManagerMenu>();
            other.managerImg.sprite = i.managerSpr;
            other.managerName.text = i.managerName + "\n" + i.name;
            //other.managerPriceText.text = i.managerCost.ToString() + "$";
            other.managerPriceText.text = FormatNumber(i.managerCost) + "$";
            other.managerbuyButton.onClick.AddListener(delegate { addManagerListener(i); });
            managersObjects.Add(managerItem);

            if (SaveGame.Exists("Automated" + i.name))
            {
                i.automated = SaveGame.Load<bool>("Automated" + i.name);
            }

            if (i.automated)
            {
                managerItem.SetActive(false);
                if (i.owned > 0 && !i.isRunning)
                {
                    StartCoroutine(RevenueCoroutine(i));
                } 
            }
        }
    }

    public void addManagerListener(items i)
    {
        if (money >= i.managerCost)
        {
            money -= i.managerCost;
            i.automated = true;

            //Savegame test
            SaveGame.Save<bool>("Automated" + i.name, i.automated); //Save whether it is automated or not. This will automatically save/load bought managers too

            if (i.owned > 0) //starts if actually has an item
            {
                StartCoroutine(RevenueCoroutine(i));
            }

            setManagers();
            moneyPerScalc();

            if (activeTutorial == 6)
            {
                menu.CloseAll();
                tut6.SetActive(true);
            }
        }
        else
        {
            Debug.Log("not enough moneys");
        }
    }

    public void closeTut()
    {
        activeTutorial = 999;
        tut6.SetActive(false);
    }

    public void setiMoneyText(items i)
    {
        float moneyToEarn = moneycalc(i);

        if (moneyToEarn > 0)
        {
            if (i.initialTime / i.timeMultiplier > 0.5f)
            {
                /*if (moneyToEarn < 100) //decide whether to set comma and two digits or not
                {
                    i.moneyPerSecondText.text = moneyToEarn.ToString("F2") + "$";
                }
                else
                {*/
                    //i.moneyPerSecondText.text = moneyToEarn.ToString("F0") + "$";
                    i.moneyPerSecondText.text = FormatNumber(moneyToEarn) + "$";
                //}
            }
            else
            {
                float moneyPerS = moneyToEarn / (i.initialTime / i.timeMultiplier);

                /*if (moneyPerS < 100)
                {
                    i.moneyPerSecondText.text = (moneyToEarn / (i.initialTime / i.timeMultiplier)).ToString("F2") + "$/s";
                }
                else
                {*/
                    i.moneyPerSecondText.text = FormatNumber((moneyToEarn / (i.initialTime / i.timeMultiplier))) + "$/s";
                //}
            }
        }
        else
        {
            i.moneyPerSecondText.text = ""; //Has no units so no text? Or should I write "0" or "None" ?
        }
    }

    public void moneyPerScalc()
    {
        moneyperS1 = 0;
        foreach (items i in totalItems)
        {
            if (i.automated)
            {
                float moneyTotal = moneycalc(i);
                float perS = moneyTotal / (i.initialTime / i.timeMultiplier);
                moneyperS1 += perS;
            }
        }
        moneyperS2 = moneyperS1;
        //moneyPerSText.text = moneyperS2.ToString("F0") + "$/s";
        moneyPerSText.text = FormatNumber(moneyperS2) + "$/s";
    }

    public void SetMoneyText()
    {
        /*if (money < 100)
        {
            moneyText.text = money.ToString("F2") + "$";
        }
        else*/
        {
            //moneyText.text = money.ToString(Double2dec(money)) + "$";
            moneyText.text = FormatNumber(money) + "$";
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

    float moneycalc(items i)
    {
        float addedMoney1 = 0;
        if (i.owned > 0)
        {
            addedMoney1 = i.initialRevenue * i.owned * multiplier * multiplierUpgradeTree * i.revenueMultiplier * i.multiplierTree;
            if (i.owned >= 25)
            {
                addedMoney1 *= i.multiplier25;
            }
            if (i.owned >= 50)
            {
                addedMoney1 *= i.multiplier50;
            }
            if (i.owned >= 100)
            {
                addedMoney1 *= i.multiplier100;
            }
            if (i.owned >= 1000)
            {
                addedMoney1 *= i.multiplier1000;
            }
            if (i.owned >= 10000)
            {
                addedMoney1 *= i.multiplier10000;
            }
            if (i.owned >= 100000)
            {
                addedMoney1 *= i.multiplier100000;
            }
            if (i.owned >= 10000000)
            {
                addedMoney1 *= i.multiplier10000000;
            }
            addedMoney1 += ((respawnMultiplier * currentRespawns) / 100);
        }
        return addedMoney1;
    }

    IEnumerator RevenueCoroutine(items i)
    {
        i.isRunning = true;
        if (i.initialTime / i.timeMultiplier > 0.5f)
        {
            if (Version == 1)
            {
                i.slider.value = 0;
                i.slider2.value = 0;
            }
            if (Version == 2)
            {
                i.radialImage.fillAmount = 0;
            }
            StartCoroutine(sliderCoroutine(i));
        }
        else
        {
            if (Version == 1)
            {
                i.slider.value = i.slider.maxValue;
                i.slider2.value = i.slider2.maxValue;
            }
            if (Version == 2)
            {
                i.radialImage.fillAmount = 1;
            }
        }
        setiMoneyText(i);

        yield return new WaitForSeconds(i.initialTime / i.timeMultiplier);
       

        float moneyToAdd = moneycalc(i);
        //money += moneycalc(i);
        money += moneyToAdd;
        lifetimeMoney += moneyToAdd;
        SetMoneyText();
        RespawnCalculater();
        i.isRunning = false;
        if (Version == 1)
        {
            i.slider.value = 0;
            i.slider2.value = 0;
        }
        if (Version == 2)
        {
            i.radialImage.fillAmount = 0;
        }

        if (activeTutorial == 3)
        {
            activeTutorial = 4;
            tut3.SetActive(false);
            tut4.SetActive(true);
            money += 400;
        }

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
        if (Version == 1)
        {
            i.slider.maxValue = i.initialTime / i.timeMultiplier;
            i.slider2.maxValue = i.initialTime / i.timeMultiplier;
        }
        if (Version == 2)
        {
            //maxvalue is always 1
        }

        float animationTime = 0f;
        while (animationTime < (i.initialTime / i.timeMultiplier))
        {
            animationTime += Time.deltaTime;
            float lerpValue = animationTime / (i.initialTime / i.timeMultiplier);
            if (Version == 1)
            {
                i.slider.value = Mathf.Lerp(0f, (i.initialTime / i.timeMultiplier), lerpValue);
                i.slider2.value = Mathf.Lerp(0f, (i.initialTime / i.timeMultiplier), lerpValue);
            }
            if (Version == 2)
            {
                //calculate max
                i.radialImage.fillAmount = Mathf.Lerp(0f, 1, lerpValue);
            }

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
                //i.BuybuttonText.text = "Buy 1 - " + i.owned +  "\n" + i.costNext + "$";
                i.BuybuttonText.text = "Buy 1 - " + FormatNumber(i.owned) +  "\n" + FormatNumber(i.costNext) + "$";

            }
            if (activeBuyInt > 1 && activeBuyInt < 99998)
            {
                float nextCost = i.initialCost * ((Mathf.Pow(i.coefficient, i.owned) - Mathf.Pow(i.coefficient, (i.owned + activeBuyInt))) / (1 - i.coefficient));
                i.CanBuy = activeBuyInt;
                i.costNext = Mathf.Floor(nextCost);
                i.BuybuttonText.text = "Buy " + i.CanBuy + "- " + FormatNumber(i.owned) + "\n" + FormatNumber(i.costNext) + "$";
            }
            if (activeBuyInt == 99999)
            {
                float maxBuy = Mathf.Floor(Mathf.Log(Mathf.Pow(i.coefficient, i.owned) - (money * (1 - i.coefficient) / i.initialCost), i.coefficient) - i.owned);
                i.CanBuy = maxBuy;
                float nextCost = i.initialCost * ((Mathf.Pow(i.coefficient, i.owned) - Mathf.Pow(i.coefficient, (i.owned + i.CanBuy))) / (1 - i.coefficient));
                i.costNext = Mathf.Floor(nextCost);
                i.BuybuttonText.text = "Buy " + FormatNumber(i.CanBuy) + "- " + FormatNumber(i.owned) + "\n" + FormatNumber(i.costNext) + "$";
            }
        }
    }

    public void soundButton()
    {
        if (BGSound.isPlaying)
        {
            BGSound.Stop();
        }
        else
        {
            BGSound.Play();
        }
    }

    public void RespawnCalculater()
    {
        newRespawns = Mathf.FloorToInt(lifetimeMoney / 10000) - TotalRespawnsBought;

        /*currentOGText.text = "Active OGs: " + currentRespawns.ToString() + "\n" + "Bonus: " + (respawnMultiplier * currentRespawns).ToString() + "%";
        respawnText.text = "Restart game with " + newRespawns.ToString() + " Original Gangsters";
        upgradeOGText.text = "Current OGs: " + currentRespawns.ToString();*/

        float multi = respawnMultiplier* currentRespawns;
        currentOGText.text = "Active OGs: " + FormatNumber(currentRespawns) + "\n" + "Bonus: " + FormatNumber(multi) + "%";
        respawnText.text = "Restart game with " + FormatNumber(newRespawns) + " Original Gangsters";
        upgradeOGText.text = "Current OGs: " + FormatNumber(currentRespawns);
    }

    public void Respawn()
    {
        if (newRespawns > 1)
        {
            TotalRespawnsBought += newRespawns;
            currentRespawns += newRespawns;
            resetStats();
            menu.CloseAll();
        }
        RespawnCalculater();
    }

    public void resetStats()
    {
        money = 10;
        SetMoneyText();
        StopAllCoroutines();

        respawnMultiplier = 1;
        multiplier = 1;
        multiplierUpgradeTree = 1;

        foreach (items i in totalItems)
        {
            //reset values
            if (Version == 1)
            {
                i.slider.value = 0;
                i.slider2.value = 0;
            }
            if (Version == 2)
            {
                i.radialImage.fillAmount = 0;
            }
            i.owned = 0;
            i.automated = false;

            SaveGame.Save("Automated" + i.name, i.automated); //reset manager savegame

            i.timeMultiplier = 1;
            i.revenueMultiplier = 1;
            i.isRunning = false;

            i.hasHit25 = 0;
            i.hasHit50 = 0;
            i.hasHit100 = 0;
            i.hasHit1000 = 0;
            i.hasHit10000 = 0;
            i.hasHit100000 = 0;
            i.hasHit10000000 = 0;

            //reset text
            //i.runOnceButtonText.text = i.owned.ToString();
            setiMoneyText(i);
        }

        if (upgradeObjects.Count > 0)
        {
            foreach (GameObject u in upgradeObjects)
            {
                u.SetActive(true);
                u.GetComponent<UpgradeItem>().checkIfBought();
            }
        }

        //give first for free
        totalItems[0].owned = 1;
        //totalItems[0].runOnceButtonText.text = totalItems[0].owned.ToString();
        setManagers();
        setiMoneyText(totalItems[0]);
        moneyPerScalc();
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
            SetMoneyText();

            i.owned += i.CanBuy;

            SaveGame.Save("owned" + i.name, i.owned);

            if (activeTutorial == 2)
            {
                tut2.SetActive(false);
                tut3.SetActive(true);
                activeTutorial = 3;
            }

            if (i.hasHit25 == 0 && i.owned >= 25)
            {
                i.hasHit25 = 1;
                PlayerPrefs.SetInt(i.name + ".hasHit25", i.hasHit25);
                PlayerPrefs.Save();
                achievementSound.Play();
            }
            else if (i.hasHit50 == 0 && i.owned >= 50)
            {
                i.hasHit50 = 1;
                PlayerPrefs.SetInt(i.name + ".hasHit50", i.hasHit50);
                PlayerPrefs.Save();
                achievementSound.Play();
            }
            else if(i.hasHit100 == 0 && i.owned >= 100)
            {
                i.hasHit100 = 1;
                PlayerPrefs.SetInt(i.name + ".hasHit100", i.hasHit100);
                PlayerPrefs.Save();
                achievementSound.Play();
            }
            else if(i.hasHit1000 == 0 && i.owned >= 1000)
            {
                i.hasHit1000 = 1;
                PlayerPrefs.SetInt(i.name + ".hasHit1000", i.hasHit1000);
                PlayerPrefs.Save();
                achievementSound.Play();
            }
            else if(i.hasHit10000 == 0 && i.owned >= 10000)
            {
                i.hasHit10000 = 1;
                PlayerPrefs.SetInt(i.name + ".hasHit10000", i.hasHit10000);
                PlayerPrefs.Save();
                achievementSound.Play();
            }
            else if(i.hasHit100000 == 0 && i.owned >= 100000)
            {
                i.hasHit100000 = 1;
                PlayerPrefs.SetInt(i.name + ".hasHit100000", i.hasHit100000);
                PlayerPrefs.Save();
                achievementSound.Play();
            }
            else if(i.hasHit10000000 == 0 && i.owned >= 10000000)
            {
                i.hasHit10000000 = 1;
                PlayerPrefs.SetInt(i.name + ".hasHit10000000", i.hasHit10000000);
                PlayerPrefs.Save();
                achievementSound.Play();
            }
            else
            {
                coinSound.Play();
            }

            if (i.automated && !i.isRunning) //if already has a manager but not running yet
            {
                StartCoroutine(RevenueCoroutine(i));
            }

            setiMoneyText(i);
            moneyPerScalc();
            nextCostCalculator();
        }
    }

    public static readonly Dictionary<int, string> Numberunits = new Dictionary<int, string>
    {
        {0, ""},
        {1, "K"},
        {2, "M"},
        {3, "B"},
        {4, "T"}
    };

    public static string FormatNumber(double value)
    {
        if (value < float.MaxValue)
        {

            if (value < 1d)
            {
                return "0";
            }

            var n = (int)Math.Log(value, 1000);
            var m = value / Math.Pow(1000, n);
            var unit = "";

            if (n < Numberunits.Count)
            {
                unit = Numberunits[n];
            }
            else
            {
                var unitInt = n - Numberunits.Count;
                var secondUnit = unitInt % 26;
                var firstUnit = unitInt / 26;
                unit = Convert.ToChar(firstUnit + charA).ToString() + Convert.ToChar(secondUnit + charA).ToString();
            }

            // Math.Floor(m * 100) / 100) fixes rounding errors
            //return (Math.Floor(m * 100) / 100).ToString("0.##") + unit;
            return (Math.Floor(m * 100) / 100).ToString("0") + unit;
        }
        else
        {
            return "MAX";
        }
    }

}

/// <summary>
///start
/// </summary>

/*i.nameText = other.nameText;
            i.nameText.text = i.name;


            //GameObject item = Instantiate(Resources.Load("Item5"), transform.position, Quaternion.identity) as GameObject;
            //item.transform.SetParent(i.spawnPos.transform, false);
            //item.transform.localPosition = new Vector3(0, 0, 0);
            //itemInfo other = i.item.GetComponent<itemInfo>();

if (Version == 1)
{
    i.slider = other.slider;
    i.slider.value = 0;
    i.slider.maxValue = i.initialTime / i.timeMultiplier;
    i.slider2 = other.slider2;
    i.slider2.value = 0;
    i.slider2.maxValue = i.initialTime / i.timeMultiplier;
}*/
//i.runOnceButtonText = other.runOnceButtonText;
//i.runOnceButtonText.text = i.owned.ToString();


//float moneyTotal = i.initialRevenue * i.owned * multiplier * multiplierUpgradeTree * i.revenueMultiplier * i.multiplierTree * i.multiplier25 * i.multiplier50 * i.multiplier100 * i.multiplier1000 * i.multiplier10000 * i.multiplier100000 * i.multiplier10000000 + ((respawnMultiplier * currentRespawns) / 100);

///
/// MoneyCalc 
///
/*float addedMoney1 = i.initialRevenue * i.owned * multiplier * multiplierUpgradeTree * i.revenueMultiplier * i.multiplierTree * i.multiplier50 * i.multiplier100 * i.multiplier1000 * i.multiplier10000 * i.multiplier100000 * i.multiplier10000000 + ((respawnMultiplier * currentRespawns) / 100);
       if (i.owned >= 25)
       {
           addedMoney1 *= i.multiplier25;
       }
       if (i.owned >= 50)
       {
           addedMoney1 *= i.multiplier50;
       }
       if (i.owned >= 100)
       {
           addedMoney1 *= i.multiplier100;
       }
       if (i.owned >= 1000)
       {
           addedMoney1 *= i.multiplier1000;
       }
       if (i.owned >= 10000)
       {
           addedMoney1 *= i.multiplier10000;
       }
       if (i.owned >= 100000)
       {
           addedMoney1 *= i.multiplier100000;
       }
       if (i.owned >= 10000000)
       {
           addedMoney1 *= i.multiplier10000000;
       }*/

