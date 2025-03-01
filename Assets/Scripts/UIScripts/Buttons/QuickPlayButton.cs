using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickPlayButton : MonoBehaviour
{
    [SerializeField]
    private TableController tableController;
    private int quickBet;
    private void OnEnable()
    {
        if (tableController.TableSettings.MinBet == 0)
        {
            if(ExchangeManager.Instance.GetCurrency(CurrencyType.Cash)>=Mathf.Round(tableController.TableSettings.MaxBet/2))
                quickBet = tableController.TableSettings.MaxBet / 2;
            else
                quickBet = ExchangeManager.Instance.GetCurrency(CurrencyType.Cash);
        }
        else
            quickBet = tableController.TableSettings.MinBet;
    }
    public void StartGame()
    {
        ExchangeManager.Instance.UseCurrency(CurrencyType.Cash, quickBet);
        EventManager.TriggerGameStart(2, quickBet);
    }

    public void OnClick()
    {
        EventManager.TriggerGameStart(2, 100);
    }
}
