using UnityEngine;
using System;
using System.Collections.Generic;

public static class EventManager
{
    #region Game Flow Events
    public static Action OnRoundReady;
    public static Action OnRoundEnd;
    public static Action OnCalculateScore;
    public static Action OnGameEnd;
    public static Action OnGameStatusChanged;
    public static Action<int, int> OnGameStart;
    public static Action<bool> OnPlayerWin;
    public static Action<bool> OnPlayerLose;
    public static Action<List<BaseController>> OnScoreBoard;
    #endregion

    #region Card Events
    public static Action<List<Card>> OnCardsCollected;
    public static Action<int> OnScoreUpdated;
    public static Action OnCardPlayed;
    #endregion

    #region Currency Events
    public static Action OnCurrencyAdded;
    public static Action<Dictionary<CurrencyType, int>> OnCurrencyChange;
    #endregion

    #region Game Flow Methods
    public static void TriggerRoundReady()
    {
        OnRoundReady?.Invoke();
    }

    public static void TriggerRoundEnd()
    {
        OnRoundEnd?.Invoke();
    }

    public static void TriggerCalculateScore()
    {
        OnCalculateScore?.Invoke();
    }

    public static void TriggerGameEnd()
    {
        OnGameEnd?.Invoke();
    }

    public static void TriggerGameStatusChanged()
    {
        OnGameStatusChanged?.Invoke();
    }

    public static void TriggerGameStart(int playerCount, int bet)
    {
        OnGameStart?.Invoke(playerCount, bet);
    }

    public static void TriggerPlayerWin(bool isWin)
    {
        OnPlayerWin?.Invoke(isWin);
    }

    public static void TriggerPlayerLose(bool isLose)
    {
        OnPlayerLose?.Invoke(isLose);
    }

    public static void TriggerScoreBoard(List<BaseController> controllers)
    {
        OnScoreBoard?.Invoke(controllers);
    }
    #endregion

    #region Card Methods
    public static void TriggerCardsCollected(List<Card> cards)
    {
        OnCardsCollected?.Invoke(cards);
    }

    public static void TriggerScoreUpdated(int score)
    {
        OnScoreUpdated?.Invoke(score);
    }

    public static void TriggerCardPlayed()
    {
        OnCardPlayed?.Invoke();
    }
    #endregion

    #region Currency Methods
    public static void TriggerCurrencyAdded()
    {
        OnCurrencyAdded?.Invoke();
    }

    public static void TriggerCurrencyChange(Dictionary<CurrencyType, int> currencies)
    {
        OnCurrencyChange?.Invoke(currencies);
    }
    #endregion
} 