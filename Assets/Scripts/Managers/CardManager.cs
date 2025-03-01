using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class CardManager : Singleton<CardManager>
{
    #region Fields
    [Header("Card Setup")]
    public GameObject cardPrefab;
    public CardDealer cardHolder;
    public Transform playGroundTransform;
    public float cardMoveSpeed = 0.5f;

    [Header("Card Sprites")]
    [SerializeField] private List<Sprite> heartsCards;
    [SerializeField] private List<Sprite> clubsCards;
    [SerializeField] private List<Sprite> diamondsCards;
    [SerializeField] private List<Sprite> spadesCards;

    private List<Card> deck;
    #endregion

    #region Initialization
    private void Start()
    {
        if (cardHolder.gameObject.activeInHierarchy)
        {
            InitializeDeck();
        }
    }

    public void InitializeDeck()
    {
        CreateDeck();
        ShuffleDeck();
        InitializeCardDealer();
    }

    private void CreateDeck()
    {
        deck = new List<Card>();
        for (int type = 0; type < (int)CardTypes.NUMBER_TYPES; type++)
        {
            for (int value = 1; value <= 13; value++)
            {
                CreateSingleCard((CardTypes)type, value);
            }
        }
    }

    private void CreateSingleCard(CardTypes type, int value)
    {
        int points = CalculateCardPoints(value, (int)type);
        GameObject cardObject = Instantiate(cardPrefab, cardHolder.cardParent);
        Card card = cardObject.GetComponent<Card>();
        card.Init(type, value, points, playGroundTransform);
        deck.Add(card);
    }
    #endregion

    #region Card Management
    private void ShuffleDeck()
    {
        System.Random random = new System.Random();
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            Card temp = deck[n];
            deck[n] = deck[k];
            deck[k] = temp;
        }
    }

    public void DealCard()
    {
        if (cardHolder.cards.Count == 0)
        {
            EventManager.TriggerCalculateScore();
            return;
        }
        cardHolder.DealToPlayers();
    }

    private void InitializeCardDealer()
    {
        cardHolder.PlayersHolders.Clear();
        foreach (var player in GameManager.Instance.currentPlayers)
        {
            cardHolder.PlayersHolders.Add(player.deckParent);
        }
        cardHolder.cards = deck;
        cardHolder.Initialize();
    }
    #endregion

    #region Card Properties
    private int CalculateCardPoints(int value, int type)
    {
        return value switch
        {
            1 => 1,
            2 when type == 3 => 2,
            10 when type == 2 => 3,
            11 => 1,
            _ => 0
        };
    }

    public Sprite GetCardSprite(int value, CardTypes type)
    {
        var spriteList = GetSpriteListByType(type);
        if (spriteList == null || value < 1 || value > spriteList.Count)
        {
            return null;
        }
        return spriteList[value - 1];
    }

    private List<Sprite> GetSpriteListByType(CardTypes type)
    {
        return type switch
        {
            CardTypes.Spades => spadesCards,
            CardTypes.Hearts => heartsCards,
            CardTypes.Diamonds => diamondsCards,
            CardTypes.Clubs => clubsCards,
            _ => null
        };
    }
    #endregion
}
