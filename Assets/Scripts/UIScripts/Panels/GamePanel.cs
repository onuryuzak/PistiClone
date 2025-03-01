using UnityEngine;

public class GamePanel : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.OnScoreBoard += HidePanel;
    }

    private void OnDisable()
    {
        EventManager.OnScoreBoard -= HidePanel;
    }

    private void HidePanel(System.Collections.Generic.List<BaseController> obj)
    {
        gameObject.SetActive(false);
    }
}
