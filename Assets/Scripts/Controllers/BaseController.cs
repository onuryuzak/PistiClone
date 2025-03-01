using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    public PlayerTypes playerType;
    public Transform collectedDeckParent;
    public Transform deckParent;

    [HideInInspector]
    public List<Card> currentDeck = new List<Card>();
    [HideInInspector]
    public GameLogic gameLogic;
    [HideInInspector]
    public bool canPlay;
    [HideInInspector]
    public int point;

    public virtual void CardAdd(Card addThis)
    {
        currentDeck.Add(addThis);
        addThis.gameObject.transform.localRotation = Quaternion.identity;
    }

    public virtual void RemoveCard(Card removeThis)
    {
        currentDeck.Remove(removeThis);
        CanClickOff();
    }

    public virtual void CanClickOff()
    {
        for (int i = 0; i < currentDeck.Count; i++)
        {
            currentDeck[i].canBeClicked = false;
        }
        canPlay = false;
    }

    public abstract void CanClickOn();
} 