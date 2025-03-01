using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TableController : MonoBehaviour
{
    public TableSettings TableSettings;
    public List<Button> buttons;
   
    [SerializeField]
    private TextMeshProUGUI betTMP;
    [SerializeField]
    private TextMeshProUGUI tableNameTMP;

    private void Start()
    {
        if (ExchangeManager.Instance.GetCurrency(CurrencyType.Cash) < TableSettings.MinBet)
            CloseAllButtons();
        else
            OpenAllButtons();
        TableSet();
    }
    private void TableSet()
    {
        betTMP.text = "Bet Range: "+TableSettings.MinBet + "-" + TableSettings.MaxBet;
        tableNameTMP.text = TableSettings.TableName;
    }
    private void CloseAllButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].interactable = false;
        }
    }
    private void OpenAllButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].interactable = true;
        }
    }
}
