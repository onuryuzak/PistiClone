using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCreateButton : MonoBehaviour
{
    private TableCreator tableCreator;
    int playerCount=2;
    private int bet;
    public void SetTableCreatorParent(TableCreator creator)
    {
        tableCreator = creator;
        bet = tableCreator.currentBet;
        playerCount = tableCreator.currentPlayerCount;
        StartGame();
    }
    private void StartGame()
    {
        ExchangeManager.Instance.UseCurrency(CurrencyType.Cash, bet);
        EventManager.TriggerGameStart(playerCount, bet);
    }

    public void OnClick(int playerCount)
    {
        EventManager.TriggerGameStart(playerCount, 100);
    }
}
