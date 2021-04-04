using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;

public class Menus : MonoBehaviour
{
    public bool menuOpen;
    public bool buyMenuOpen;

    public GameObject upgradeBtn;
    public GameObject ManagerBtn;
    public GameObject OGBtn;

    public GameObject OGPanel;
    public GameObject ManagerPanel;
    public GameObject UpgradePanel;

    public GameObject[] buyButtons;
    public GameObject bulkBuyBtn;

    public bool TutorialActive;
    public Inventory2Script inven;

    private void Start()
    {
        CloseAll();
    }

    public void startBuying()
    {
        if (!menuOpen && !buyMenuOpen)
        {
            CloseAll();
            foreach (GameObject but in buyButtons)
            {
                but.SetActive(true);
            }
            bulkBuyBtn.SetActive(true);
            buyMenuOpen = true;

            if (TutorialActive)
            {
                if (inven.activeTutorial == 1)
                {
                    inven.activeTutorial = 2;
                    inven.tut1.SetActive(false);
                    inven.tut2.SetActive(true);
                }
            }
        }
        else if (!menuOpen && buyMenuOpen)
        {
            CloseAll();
        }
        else if (menuOpen && !buyMenuOpen)
        {
            CloseAll();
            foreach (GameObject but in buyButtons)
            {
                but.SetActive(true);
            }
            bulkBuyBtn.SetActive(true);
            buyMenuOpen = true;

            if (TutorialActive)
            {
                if (inven.activeTutorial == 1)
                {
                    inven.activeTutorial = 2;
                    inven.tut1.SetActive(false);
                    inven.tut2.SetActive(true);
                }
            }
        }
    }

    public void openMenu()
    {
        if (!menuOpen && !buyMenuOpen)
        {
            CloseAll();
            upgradeBtn.SetActive(true);
            ManagerBtn.SetActive(true);
            OGBtn.SetActive(true);
            menuOpen = true;

            if (TutorialActive)
            {
                if (inven.activeTutorial == 4)
                {
                    inven.activeTutorial = 5;
                    inven.tut4.SetActive(false);
                    inven.tut5.SetActive(true);
                }
            }
        }
        else if (menuOpen && !buyMenuOpen)
        {
            CloseAll();
        }
        else if (!menuOpen && buyMenuOpen)
        {
            CloseAll();
            upgradeBtn.SetActive(true);
            ManagerBtn.SetActive(true);
            OGBtn.SetActive(true);
            menuOpen = true;

            if (TutorialActive)
            {
                if (inven.activeTutorial == 4)
                {
                    inven.activeTutorial = 5;
                    inven.tut4.SetActive(false);
                    inven.tut5.SetActive(true);
                }
            }
        }

    }

    public void CloseAll()
    {
        upgradeBtn.SetActive(false);
        ManagerBtn.SetActive(false);
        OGBtn.SetActive(false);
        OGPanel.SetActive(false);
        ManagerPanel.SetActive(false);
        UpgradePanel.SetActive(false);
        bulkBuyBtn.SetActive(false);
        foreach (GameObject but in buyButtons)
        {
            but.SetActive(false);
        }
        menuOpen = false;
        buyMenuOpen = false;
    }

    public void openOGmenu()
    {
        CloseAll();
        menuOpen = true;
        OGPanel.SetActive(true);
    }

    public void OpenManagerPanel()
    {
        CloseAll();
        menuOpen = true;
        ManagerPanel.SetActive(true);

        if (TutorialActive)
        {
            if (inven.activeTutorial == 5)
            {
                inven.activeTutorial = 6;
                inven.tut5.SetActive(false);
            }
        }
    }

    public void OpenUpgradePanel()
    {
        CloseAll();
        menuOpen = true;
        UpgradePanel.SetActive(true);
    }

    public void ResetSavegame()
    {
        SaveGame.DeleteAll();
        PlayerPrefs.DeleteAll();
    }

}
