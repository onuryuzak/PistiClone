using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoSet : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerName;
    [SerializeField]
    private TextMeshProUGUI playerCash;

    private void OnEnable()
    {
        EventManager.OnCurrencyChange += SetInfo;
    }

    private void OnDisable()
    {
        EventManager.OnCurrencyChange -= SetInfo;
    }

    private void SetInfo(Dictionary<CurrencyType,int> newCurrency)
    {
        PlayerData playerData = SaveSystem.LoadPlayerData("Player");
        playerName.text = playerData.playerName;
        playerCash.text = newCurrency[CurrencyType.Cash].ToString();
    }
}
