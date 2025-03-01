using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    #region Fields
    [Header("Game State")]
    [SerializeField] private float cardCollectionDelay = 0.5f;
    [HideInInspector] public List<Card> playedCards = new List<Card>();
    [HideInInspector] public List<BaseController> Players = new List<BaseController>();
    [HideInInspector] public int playIndex;

    private int playerCount;
    private int currentTurn;
    private int lastCollectIndex;
    private bool isRoundReady;
    #endregion

    #region Initialization
    public void Initialize(int currentPlayerCount, List<BaseController> currentPlayers)
    {
        if (!ValidateInitialization(currentPlayerCount, currentPlayers)) return;

        SetupGame(currentPlayerCount, currentPlayers);
        InitializePlayers();
    }

    private bool ValidateInitialization(int currentPlayerCount, List<BaseController> currentPlayers)
    {
        if (currentPlayers == null || currentPlayers.Count == 0)
        {
            Debug.LogError("Invalid players list in GameLogic.Initialize!");
            return false;
        }

        if (currentPlayers.Count != currentPlayerCount)
        {
            Debug.LogError($"Player count mismatch! Expected: {currentPlayerCount}, Actual: {currentPlayers.Count}");
            return false;
        }

        return true;
    }

    private void SetupGame(int currentPlayerCount, List<BaseController> currentPlayers)
    {
        playerCount = currentPlayerCount;
        playIndex = Random.Range(0, playerCount);
        Players = new List<BaseController>(currentPlayers);
        playedCards ??= new List<Card>();
        playedCards.Clear();
    }

    private void InitializePlayers()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i] == null)
            {
                Debug.LogError($"Player at index {i} is null!");
                return;
            }
            Players[i].gameLogic = this;
        }
    }
    #endregion

    #region Event Handlers
    private void OnEnable()
    {
        EventManager.OnRoundReady += ReadyToPlay;
        EventManager.OnCalculateScore += CalculateScore;
    }

    private void OnDisable()
    {
        EventManager.OnRoundReady -= ReadyToPlay;
        EventManager.OnCalculateScore -= CalculateScore;
    }
    #endregion

    #region Game Flow
    public void AddPlayedCard(Card card)
    {
        if (card == null) return;

        AddCardToPlayedCards(card);
        NotifyAIPlayers(card);
        ProcessPlayedCard();
    }

    private void AddCardToPlayedCards(Card card)
    {
        card.transform.SetParent(transform, false);
        playedCards.Add(card);
    }

    private void NotifyAIPlayers(Card card)
    {
        foreach (BaseController player in Players)
        {
            if (player is AIControl aiControl)
            {
                aiControl.UpdateMemory(card);
            }
        }
    }

    private void ProcessPlayedCard()
    {
        if (!isRoundReady) return;
        
        if (playedCards.Count > 1)
        {
            CalculateWinLose();
        }
        else
        {
            NextPlayer();
        }
    }

    private void NextPlayer()
    {
        UpdatePlayIndex();
        UpdateTurnCount();
    }

    private void UpdatePlayIndex()
    {
        playIndex = (playIndex + 1) % playerCount;
    }

    private void UpdateTurnCount()
    {
        currentTurn++;
        if (currentTurn == playerCount * 4)
        {
            CardManager.Instance.DealCard();
            currentTurn = 0;
        }
        else
        {
            ReadyToPlay();
        }
    }

    private void ReadyToPlay()
    {
        var currentPlayer = Players[playIndex];
        if (!ValidateCurrentPlayer(currentPlayer)) return;

        isRoundReady = true;
        currentPlayer.canPlay = true;
        currentPlayer.CanClickOn();
        
        Debug.Log($"ReadyToPlay: Player {playIndex} Type: {currentPlayer.playerType} with {currentPlayer.currentDeck.Count} cards");
    }

    private bool ValidateCurrentPlayer(BaseController player)
    {
        if (player == null)
        {
            Debug.LogError($"Current player at index {playIndex} is null!");
            return false;
        }

        if (player.currentDeck == null)
        {
            Debug.LogError($"Current deck for player {player.playerType} is null!");
            return false;
        }

        if (player.currentDeck.Count == 0)
        {
            Debug.LogWarning($"Current deck for player {player.playerType} is empty. Dealing new cards...");
            CardManager.Instance.DealCard();
            return false;
        }

        return true;
    }
    #endregion

    #region Win/Lose Calculation
    private void CalculateWinLose()
    {
        if (playedCards.Count < 2) return;

        Card previousCard = playedCards[playedCards.Count - 2];
        Card currentCard = playedCards[playedCards.Count - 1];

        if (IsPisti(previousCard, currentCard))
        {
            AwardPistiPoints(previousCard, currentCard);
            StartCoroutine(CollectCardsCoroutine());
        }
        else if (IsMatchingCards(previousCard, currentCard))
        {
            StartCoroutine(CollectCardsCoroutine());
        }
        else
        {
            NextPlayer();
        }
    }

    private bool IsPisti(Card previousCard, Card currentCard)
    {
        return playedCards.Count == 2 && previousCard.Value == currentCard.Value;
    }

    private void AwardPistiPoints(Card previousCard, Card currentCard)
    {
        previousCard.PistiPoint();
        currentCard.PistiPoint();
    }

    private bool IsMatchingCards(Card previousCard, Card currentCard)
    {
        return previousCard.Value == currentCard.Value || currentCard.Value == 11;
    }

    private IEnumerator CollectCardsCoroutine()
    {
        yield return new WaitForSeconds(cardCollectionDelay);
        MoveAllCardsToWinner(playIndex);
        NextPlayer();
    }

    private void MoveAllCardsToWinner(int winnerIndex)
    {
        if (winnerIndex < 0 || winnerIndex >= Players.Count) return;

        var cardCollector = Players[winnerIndex].collectedDeckParent.GetComponent<CardCollector>();
        if (cardCollector == null)
        {
            Debug.LogError($"CardCollector component not found on player {winnerIndex}'s collected deck parent");
            return;
        }

        foreach (Card card in playedCards)
        {
            cardCollector.AddCard(card);
            card.MoveToCollectedDecks(Players[winnerIndex].collectedDeckParent);
            NotifyAIPlayers(card);
        }

        lastCollectIndex = winnerIndex;
        playedCards.Clear();
    }

    private void CalculateScore()
    {
        if (playedCards.Count > 0 && lastCollectIndex >= 0 && lastCollectIndex < Players.Count)
        {
            MoveAllCardsToWinner(lastCollectIndex);
        }

        DetermineWinner();
    }

    private void DetermineWinner()
    {
        int maxCount = 0;
        int maxIndex = -1;

        for (int i = 0; i < Players.Count; i++)
        {
            var collector = Players[i].collectedDeckParent.GetComponent<CardCollector>();
            if (collector == null) continue;
            
            int cardCount = collector.CollectedCards.Count;
            if (cardCount > maxCount)
            {
                maxCount = cardCount;
                maxIndex = i;
            }
        }

        if (maxIndex >= 0)
        {
            var winnerCollector = Players[maxIndex].collectedDeckParent.GetComponent<CardCollector>();
            if (winnerCollector != null)
            {
                winnerCollector.HasMoreCard();
            }
        }

        EventManager.TriggerGameEnd();
    }
    #endregion
}