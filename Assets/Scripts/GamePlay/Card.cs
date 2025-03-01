using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    #region Properties
    public bool canBeClicked;
    public Image frontSide;
    public GameObject backSide;
    
    private int point;
    private Transform playGroundHolder;
    private CardTypes type;
    private int value;
    private BaseController currentController;

    public int Value
    {
        get => value;
        private set => this.value = value;
    }
    #endregion

    #region Initialization
    public void Init(CardTypes cardType, int cardValue, int cardPuan, Transform playGround)
    {
        type = cardType;
        Value = cardValue;
        point = cardPuan;
        playGroundHolder = playGround;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (frontSide != null)
        {
            frontSide.sprite = CardManager.Instance.GetCardSprite(Value, type);
        }
    }
    #endregion

    #region Card Movement
    public void Move(Transform targetTransform, float moveSpeed, bool isReversed)
    {
        transform.DOMove(targetTransform.position, moveSpeed)
            .OnComplete(() => HandleMoveComplete(isReversed, targetTransform))
            .SetLink(gameObject);
    }

    private void HandleMoveComplete(bool isReversed, Transform targetTransform)
    {
        ReverseCard(isReversed);
        HandleDeckAssignment(targetTransform);
    }

    public void MoveToCollectedDecks(Transform targetTransform)
    {
        transform.DOMove(targetTransform.position, 0.2f)
            .OnComplete(() => {
                transform.SetParent(targetTransform);
                currentController = null;
            })
            .SetLink(gameObject);
    }

    private void HandleDeckAssignment(Transform parentTransform)
    {
        transform.SetParent(parentTransform);
        
        if (parentTransform == playGroundHolder)
        {
            var gameLogic = playGroundHolder.GetComponent<GameLogic>();
            if (gameLogic != null)
            {
                RemoveFromCurrentDeck(); // Remove from previous deck before adding to playground
                gameLogic.AddPlayedCard(this);
            }
            return;
        }

        var controller = parentTransform.GetComponentInParent<BaseController>();
        if (controller != null)
        {
            currentController = controller;
            controller.CardAdd(this);
        }
        else
        {
            Debug.LogWarning($"No BaseController found for card parent: {parentTransform.name}");
        }
    }
    #endregion

    #region Card Actions
    public void ClickCard()
    {
        if (!canBeClicked) return;
        
        canBeClicked = false;
        PlayCard();
    }

    public void UseCard()
    {
        PlayCard();
    }

    private void PlayCard()
    {
        const float moveSpeed = 0.5f;
        RemoveFromCurrentDeck(); // Remove before moving
        Move(playGroundHolder, moveSpeed, false);
        ApplyRandomRotation(moveSpeed);
    }

    private void ApplyRandomRotation(float rotateTime)
    {
        float randomAngle = Random.Range(-180f, 180f);
        transform.DORotate(new Vector3(0f, 0f, randomAngle), rotateTime, RotateMode.Fast)
            .SetEase(Ease.Linear);
    }

    private void RemoveFromCurrentDeck()
    {
        if (currentController != null)
        {
            currentController.RemoveCard(this);
        }
        else
        {
            var parentController = GetComponentInParent<BaseController>();
            if (parentController != null)
            {
                parentController.RemoveCard(this);
            }
            else
            {
                Debug.LogWarning($"Card {gameObject.name} is not associated with any controller");
            }
        }
    }
    #endregion

    #region Card State
    public void ReverseCard(bool reverse)
    {
        if (backSide != null)
        {
            backSide.SetActive(reverse);
        }
        if (frontSide != null)
        {
            frontSide.gameObject.SetActive(!reverse);
        }
    }

    public void PistiPoint()
    {
        point += 5;
    }

    public int GetPoint() => point;
    #endregion

    private void OnDestroy()
    {
        DOTween.Kill(transform);
        currentController = null;
    }
}