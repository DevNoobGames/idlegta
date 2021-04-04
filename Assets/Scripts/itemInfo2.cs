using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class itemInfo2 : MonoBehaviour
{
    public Image radialImage;
    public Button Buybutton;
    public Button runOnceButton; //this
    public TextMeshProUGUI BuybuttonText;
    //public TextMeshProUGUI runOnceButtonText;
    //public TextMeshProUGUI nameText;
    public TextMeshProUGUI moneyPerSecondText;

    public Image theButton;

    void Start()
    {
        if (theButton != null)
        {
            theButton.alphaHitTestMinimumThreshold = 0.2f;
        }
    }
}
