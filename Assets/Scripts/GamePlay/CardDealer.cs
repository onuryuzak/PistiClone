using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardDealer : MonoBehaviour
{
    #region Fields
    [Header("References")]
    public Transform playGroundHolder;
    public TextMeshProUGUI cardCountText;
    public List<Transform> PlayersHolders;
    public GameLogic gameLogic;
    public Transform cardParent;

    [Header("Cards")]
    public List<Card> cards;

    [Header("Timing")]
    private const float CARD_DEAL_DELAY = 0.2f;
    private const float INITIAL_DEAL_DELAY = 0.1f;
    private const float DEAL_END_DELAY = 1f;
    private const int INITIAL_GROUND_CARDS = 3;
    private const int CARDS_PER_PLAYER = 4;
    #endregion

    public void Initialize()
    {
        UpdateCardCountDisplay();
        ArrangeDealOrder();
        StartInitialDeal();
    }

    private void UpdateCardCountDisplay()
    {
        if (cardCountText != null)
        {
            cardCountText.text = cards.Count.ToString();
        }
    }

    private void ArrangeDealOrder()
    {
        List<Transform> tempHolders = new List<Transform>();
        for (int i = gameLogic.playIndex; i < PlayersHolders.Count; i++)
        {
            tempHolders.Add(PlayersHolders[i]);
        }
        for (int i = 0; i < gameLogic.playIndex; i++)
        {
            tempHolders.Add(PlayersHolders[i]);
        }
        PlayersHolders = tempHolders;
    }

    private void StartInitialDeal()
    {
        StartCoroutine(InitialDealCoroutine());
    }

    private IEnumerator InitialDealCoroutine()
    {
        // Deal face-down cards to playground
        for (int i = 0; i < INITIAL_GROUND_CARDS; i++)
        {
            DealCardToGround(true);
            yield return new WaitForSeconds(CARD_DEAL_DELAY);
        }

        // Deal final face-up card
        DealCardToGround(false);
        yield return new WaitForSeconds(INITIAL_DEAL_DELAY);
        
        DealToPlayers();
    }

    private void DealCardToGround(bool isFaceDown)
    {
        if (cards.Count > 0)
        {
            MoveCard(cards[0], playGroundHolder, isFaceDown);
        }
    }

    public void DealToPlayers()
    {
        EventManager.TriggerRoundEnd();
        Debug.Log($"Starting to deal cards. Total cards: {cards.Count}");

        ClearPlayerDecks();
        DealCardsToPlayers();
        
        Invoke(nameof(FinishDealing), DEAL_END_DELAY);
    }

    private void ClearPlayerDecks()
    {
        foreach (Transform playerHolder in PlayersHolders)
        {
            var controller = playerHolder.GetComponentInParent<BaseController>();
            if (controller != null)
            {
                controller.currentDeck.Clear();
            }
        }
    }

    private void DealCardsToPlayers()
    {
        List<Card> remainingCards = new List<Card>(cards);

        for (int round = 0; round < CARDS_PER_PLAYER; round++)
        {
            for (int playerIndex = 0; playerIndex < PlayersHolders.Count; playerIndex++)
            {
                if (remainingCards.Count > 0)
                {
                    DealCardToPlayer(remainingCards[0], playerIndex);
                    remainingCards.RemoveAt(0);
                }
            }
        }
    }

    private void DealCardToPlayer(Card card, int playerIndex)
    {
        if (card == null) return;

        var playerHolder = PlayersHolders[playerIndex];
        var controller = playerHolder.GetComponentInParent<BaseController>();

        if (controller != null)
        {
            bool isFaceDown = controller.playerType != PlayerTypes.Player;
            controller.currentDeck.Add(card);
            MoveCard(card, playerHolder, isFaceDown);
        }
        else
        {
            Debug.LogError($"No BaseController found for player holder {playerIndex}");
        }
    }

    private IEnumerator DealCardWithDelay(Card card, Transform target, float delay, bool isFaceDown)
    {
        yield return new WaitForSeconds(delay);
        MoveCard(card, target, isFaceDown);
    }

    private void MoveCard(Card card, Transform target, bool isFaceDown)
    {
        card.Move(target, CardManager.Instance.cardMoveSpeed, isFaceDown);
        RemoveCardFromDeck(card);
    }

    private void RemoveCardFromDeck(Card card)
    {
        cards.Remove(card);
        UpdateCardCountDisplay();
    }

    private void FinishDealing()
    {
        EventManager.TriggerRoundReady();
    }
}
