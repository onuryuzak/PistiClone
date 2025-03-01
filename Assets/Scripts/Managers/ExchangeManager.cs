using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExchangeManager : Singleton<ExchangeManager>
{
    #region Params
    private Dictionary<CurrencyType, int> currencyDictionary;
    const int STARTER_COIN = 1000;
    #endregion

    #region StarterMethods
    public ExchangeManager()
    {
        currencyDictionary = new Dictionary<CurrencyType, int>();
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt(PrefsKeys.Cash, STARTER_COIN) < STARTER_COIN)
            PlayerPrefs.SetInt(PrefsKeys.Cash, STARTER_COIN);

        currencyDictionary[CurrencyType.Cash] = PlayerPrefs.GetInt(PrefsKeys.Cash, STARTER_COIN);
        EventManager.TriggerCurrencyChange(currencyDictionary);
    }

    private void OnDestroy()
    {
        base.OnDestroy();
    }

    private void OnApplicationQuit()
    {
        base.OnDestroy();
    }
    #endregion

    #region CurrencyMethods
    public bool UseCurrency(CurrencyType currencyType, int amount)
    {
        if (currencyDictionary.ContainsKey(currencyType))
        {
            if (currencyDictionary[currencyType] >= amount)
            {
                currencyDictionary[currencyType] -= amount;
                PlayerPrefs.SetInt(PrefsKeys.Cash, currencyDictionary[currencyType]);
                EventManager.TriggerCurrencyChange(currencyDictionary);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void AddCurrency(CurrencyType currencyType, int amount)
    {
        if (currencyDictionary.ContainsKey(currencyType))
        {
            currencyDictionary[currencyType] += amount;
            PlayerPrefs.SetInt(PrefsKeys.Cash, currencyDictionary[currencyType]);
            EventManager.TriggerCurrencyChange(currencyDictionary);
        }
    }

    public int GetCurrency(CurrencyType currencyType)
    {
        if (currencyDictionary.ContainsKey(currencyType))
        {
            return currencyDictionary[currencyType];
        }
        else
        {
            return 0;
        }
    }
    #endregion
}