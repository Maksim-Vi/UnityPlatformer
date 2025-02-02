using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {get; private set;}
    [SerializeField] private Text AmountCoins;
    private int _currentAmount = 0;

    private void Start() 
    {
        Instance = this;
    }

    public void SetAmountCoins(int amount)
    {
        _currentAmount += amount;
        AmountCoins.text = "Amount: " + _currentAmount.ToString();
    }
}