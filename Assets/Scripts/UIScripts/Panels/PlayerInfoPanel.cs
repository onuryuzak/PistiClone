using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cashText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;

    private void OnEnable()
    {
        EventManager.OnPlayerWin += UpdateText;
        EventManager.OnPlayerLose += UpdateText;
        UpdateText(false);
    }

    private void OnDisable()
    {
        EventManager.OnPlayerWin -= UpdateText;
        EventManager.OnPlayerLose -= UpdateText;
    }

    private void Start()
    {
        UpdateText(false);
    }

    private void UpdateText(bool isWin)
    {
        cashText.text = ExchangeManager.Instance.GetCurrency(CurrencyType.Cash).ToString();
        winText.text = PlayerPrefs.GetInt(PrefsKeys.WinCount, 0).ToString();
        loseText.text = PlayerPrefs.GetInt(PrefsKeys.LoseCount, 0).ToString();
    }
}