using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CardCollector : MonoBehaviour
{
    #region Fields
    [SerializeField] private TextMeshProUGUI scoreText;
    private List<Card> collectedCards = new List<Card>();
    private int currentScore;
    private bool shouldCalculateScore = true;
    private BaseController ownerController;
    #endregion

    #region Properties
    public List<Card> CollectedCards
    {
        get => collectedCards;
        private set => collectedCards = value ?? new List<Card>();
    }

    private BaseController OwnerController
    {
        get
        {
            if (ownerController == null)
            {
                ownerController = GetComponentInParent<BaseController>();
                if (ownerController == null)
                {
                    Debug.LogError($"No BaseController found for CardCollector: {gameObject.name}");
                }
            }
            return ownerController;
        }
    }

    public int CurrentScore
    {
        get => currentScore;
        private set
        {
            if (currentScore != value)
            {
                currentScore = value;
                EventManager.TriggerScoreUpdated(currentScore);
            }
        }
    }
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeScoreText();
    }

    private void OnEnable()
    {
        EventManager.OnCalculateScore += DisableScoreCalculation;
    }

    private void OnDisable()
    {
        EventManager.OnCalculateScore -= DisableScoreCalculation;
    }

    private void Update()
    {
        if (!shouldCalculateScore) return;
        if (collectedCards.Count != transform.childCount)
        {
            UpdateScore();
        }
    }

    private void InitializeScoreText()
    {
        if (scoreText == null)
        {
            // Önce transform hiyerarşisinde Score text'i ara
            var scoreTextObj = transform.parent?.Find("Score")?.GetComponent<TextMeshProUGUI>();
            if (scoreTextObj != null)
            {
                scoreText = scoreTextObj;
            }
            else
            {
                Debug.LogWarning($"Score text not found for CardCollector: {gameObject.name}");
            }
        }
    }
    #endregion

    #region Card Collection Management
    public void AddCard(Card card)
    {
        if (card != null && !collectedCards.Contains(card))
        {
            collectedCards.Add(card);
            EventManager.TriggerCardsCollected(collectedCards);
            UpdateScore();
        }
    }

    public void RemoveCard(Card card)
    {
        if (card != null && collectedCards.Contains(card))
        {
            collectedCards.Remove(card);
            EventManager.TriggerCardsCollected(collectedCards);
            UpdateScore();
        }
    }

    public void ClearCards()
    {
        collectedCards.Clear();
        EventManager.TriggerCardsCollected(collectedCards);
        UpdateScore();
    }
    #endregion

    #region Score Management
    public void HasMoreCard()
    {
        AddBonusPoints();
    }

    private void AddBonusPoints()
    {
        CurrentScore += 3;
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (OwnerController == null) return;

        RefreshCollectedCards();
        CalculateCurrentScore();
        UpdateScoreDisplay();
        UpdateControllerScore();
    }

    private void RefreshCollectedCards()
    {
        var previousCount = collectedCards.Count;
        collectedCards.Clear();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<Card>(out Card card))
            {
                collectedCards.Add(card);
            }
        }

        if (previousCount != collectedCards.Count)
        {
            EventManager.TriggerCardsCollected(collectedCards);
        }
    }

    private void CalculateCurrentScore()
    {
        int newScore = 0;
        foreach (var card in collectedCards)
        {
            newScore += card.GetPoint();
        }
        CurrentScore = newScore;
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText == null)
        {
            InitializeScoreText();
        }

        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {CurrentScore}";
        }
    }

    private void UpdateControllerScore()
    {
        if (OwnerController != null)
        {
            OwnerController.point = CurrentScore;
        }
    }

    private void DisableScoreCalculation()
    {
        shouldCalculateScore = false;
    }
    #endregion
}
