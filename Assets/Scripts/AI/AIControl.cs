using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIControl : BaseController
{
    
    private AIDifficultyData difficultyData;
    
    private List<Card> rememberedCards = new List<Card>();
    private int lastPlayedCardValue = -1;
    
    private void Start()
    {
        playerType = PlayerTypes.Bot1;  // Set as bot by default
    }

    public void SetDifficulty(AIDifficultyData difficulty)
    {
        difficultyData = difficulty;
    }

    public override void CanClickOn()
    {
        if (!canPlay)
        {
            Debug.LogWarning($"AI {playerType} cannot play - canPlay is false");
            return;
        }
        
        if (currentDeck == null || currentDeck.Count == 0)
        {
            Debug.LogError($"AI {playerType} has no cards in currentDeck!");
            return;
        }

        if (gameLogic == null)
        {
            Debug.LogError($"AI {playerType} gameLogic reference is null!");
            return;
        }

        if (difficultyData == null)
        {
            Debug.LogError($"AI {playerType} difficulty data is not set!");
            return;
        }

        Debug.Log($"AI {playerType} starting turn with {currentDeck.Count} cards, difficulty: {difficultyData.difficultyLevel}");
        StartCoroutine(PlayWithDelay());
    }

    private IEnumerator PlayWithDelay()
    {
        if (difficultyData == null)
        {
            Debug.LogError("AI difficulty data is not set!");
            yield break;
        }

        Debug.Log($"AI {playerType} waiting for {difficultyData.playDelay} seconds");
        yield return new WaitForSeconds(difficultyData.playDelay);
        
        PlayAITurn();
    }

    private void PlayAITurn()
    {
        if (!canPlay)
        {
            Debug.LogWarning("AI cannot play - canPlay became false during delay");
            return;
        }

        Card chosenCard = ChooseCardToPlay();
        if (chosenCard != null)
        {
            Debug.Log($"AI {playerType} playing card: Value={chosenCard.Value}");
            chosenCard.canBeClicked = true;
            chosenCard.ReverseCard(false);
            chosenCard.UseCard();
        }
        else
        {
            Debug.LogError("AI failed to choose a card!");
        }
        
        CanClickOff();
    }

    private Card ChooseCardToPlay()
    {
        // Get the last played card if any
        Card lastPlayedCard = null;
        if (gameLogic.playedCards.Count > 0)
        {
            lastPlayedCard = gameLogic.playedCards[gameLogic.playedCards.Count - 1];
            lastPlayedCardValue = lastPlayedCard.Value;
        }

        // Strategy based on difficulty level
        switch (difficultyData.difficultyLevel)
        {
            case AILevel.Hard:
                return ChooseCardHard(lastPlayedCard);
            case AILevel.Medium:
                return ChooseCardMedium(lastPlayedCard);
            case AILevel.Easy:
            default:
                return ChooseCardEasy(lastPlayedCard);
        }
    }

    private Card ChooseCardEasy(Card lastPlayedCard)
    {
        // Easy AI just plays randomly with basic matching
        if (lastPlayedCard == null)
        {
            return currentDeck[Random.Range(0, currentDeck.Count)];
        }

        // Try to match the card value
        foreach (Card card in currentDeck)
        {
            if (card.Value == lastPlayedCard.Value)
            {
                return card;
            }
        }

        // If no match, play random
        return currentDeck[Random.Range(0, currentDeck.Count)];
    }

    private Card ChooseCardMedium(Card lastPlayedCard)
    {
        // Medium AI tries to collect cards and remembers some played cards
        if (lastPlayedCard == null)
        {
            // Try to play a card that has appeared before
            foreach (Card card in currentDeck)
            {
                if (rememberedCards.Exists(c => c.Value == card.Value))
                {
                    return card;
                }
            }
            
            // Avoid playing Jack (11) as first card
            return currentDeck.Find(c => c.Value != 11) ?? currentDeck[0];
        }

        // Try to match for Pisti
        if (gameLogic.playedCards.Count == 1)
        {
            Card matchingCard = currentDeck.Find(c => c.Value == lastPlayedCard.Value);
            if (matchingCard != null) return matchingCard;
        }

        // Try to collect with Jack
        Card jack = currentDeck.Find(c => c.Value == 11);
        if (jack != null && gameLogic.playedCards.Count > 0)
        {
            return jack;
        }

        // Try to match the last card
        Card matching = currentDeck.Find(c => c.Value == lastPlayedCard.Value);
        if (matching != null) return matching;

        // Play lowest value card
        return currentDeck.OrderBy(c => c.GetPoint()).First();
    }

    private Card ChooseCardHard(Card lastPlayedCard)
    {
        // Hard AI uses advanced strategy and remembers all played cards
        if (lastPlayedCard == null)
        {
            // If we have multiple cards of the same value, save them for potential Pisti
            var groupedCards = currentDeck.GroupBy(c => c.Value)
                                        .Where(g => g.Count() > 1)
                                        .FirstOrDefault();
            
            if (groupedCards != null)
            {
                return groupedCards.First();
            }
            
            // Play strategically based on remembered cards
            foreach (Card card in currentDeck.OrderBy(c => c.GetPoint()))
            {
                if (!rememberedCards.Exists(c => c.Value == card.Value) && card.Value != 11)
                {
                    return card;
                }
            }
            
            return currentDeck.OrderBy(c => c.GetPoint()).First();
        }

        // Try to make Pisti
        if (gameLogic.playedCards.Count == 1)
        {
            Card pistiCard = currentDeck.Find(c => c.Value == lastPlayedCard.Value);
            if (pistiCard != null) return pistiCard;
        }

        // Use Jack strategically
        Card jack = currentDeck.Find(c => c.Value == 11);
        if (jack != null)
        {
            // Use Jack if there are valuable cards to collect
            bool hasValuableCards = gameLogic.playedCards.Any(c => c.GetPoint() > 0);
            if (hasValuableCards) return jack;
        }

        // Match last card if possible
        Card matching = currentDeck.Find(c => c.Value == lastPlayedCard.Value);
        if (matching != null) return matching;

        // Play strategically based on card points and remembered cards
        return currentDeck.OrderBy(c => c.GetPoint())
                         .ThenBy(c => rememberedCards.Count(rc => rc.Value == c.Value))
                         .First();
    }

    public void UpdateMemory(Card playedCard)
    {
        // Update memory based on AI's capability
        if (Random.Range(0, 100) < difficultyData.memoryCapability)
        {
            rememberedCards.Add(playedCard);
        }
    }
} 