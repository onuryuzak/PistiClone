using UnityEngine;
using TMPro;

public class PlayerController : BaseController
{
    [SerializeField]
    private TextMeshProUGUI currentCashTMP;

    private void OnEnable()
    {
        if (currentCashTMP != null)
            currentCashTMP.text = ExchangeManager.Instance.GetCurrency(CurrencyType.Cash).ToString();
    }

    public override void CardAdd(Card addThis)
    {
        base.CardAdd(addThis);
        
        if (playerType == PlayerTypes.Player)
        {
            addThis.canBeClicked = true;
        }
    }

    public override void CanClickOn()
    {
        if (!canPlay) return;

        if (playerType == PlayerTypes.Player)
        {
            for (int i = 0; i < currentDeck.Count; i++)
            {
                currentDeck[i].canBeClicked = true;
            }
        }
        canPlay = true;
    }
} 