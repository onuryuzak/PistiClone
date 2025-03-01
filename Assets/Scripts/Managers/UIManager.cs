using UnityEngine;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    #region Panel References
    [Header("Main Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject playerInfoPanel;
    [SerializeField] private GameObject createTablePanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject scoreBoardPanel;

    [Header("Player UI Elements")]
    [SerializeField] private List<GameObject> playerChairs;

    private GameObject currentActivePanel;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        ValidateReferences();
        InitializeUI();
    }

    private void ValidateReferences()
    {
        Debug.Assert(menuPanel != null, "Menu Panel is not assigned!");
        Debug.Assert(playerInfoPanel != null, "Player Info Panel is not assigned!");
        Debug.Assert(createTablePanel != null, "Create Table Panel is not assigned!");
        Debug.Assert(gamePanel != null, "Game Panel is not assigned!");
        Debug.Assert(scoreBoardPanel != null, "Score Board Panel is not assigned!");
        Debug.Assert(playerChairs != null && playerChairs.Count > 0, "Player Chairs are not assigned!");
    }

    private void InitializeUI()
    {
        DeactivateAllPanels();
        DeactivatePlayerChairs();
        ShowPanel(menuPanel);
    }
    #endregion

    #region Panel Management
    public void HandleGameStateChange(GameStatus gameState)
    {
        DeactivateAllPanels();

        switch (gameState)
        {
            case GameStatus.Menu:
                ShowPanel(menuPanel);
                break;
            case GameStatus.WaitForPlay:
            case GameStatus.Play:
                ShowPanel(gamePanel);
                break;
            case GameStatus.ScoreBoard:
                ShowPanel(scoreBoardPanel);
                break;
            default:
                Debug.LogWarning($"Unhandled game state: {gameState}");
                break;
        }
    }

    private void DeactivateAllPanels()
    {
        menuPanel.SetActive(false);
        playerInfoPanel.SetActive(false);
        createTablePanel.SetActive(false);
        gamePanel.SetActive(false);
        scoreBoardPanel.SetActive(false);
    }

    private void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            currentActivePanel = panel;
            panel.SetActive(true);
        }
    }

    public void ShowCreateTablePanel(int minBet, int maxBet)
    {
        if (createTablePanel == null) return;

        var tableCreator = createTablePanel.GetComponent<TableCreator>();
        if (tableCreator != null)
        {
            DeactivateAllPanels();
            ShowPanel(createTablePanel);
            tableCreator.BetSetter(minBet, maxBet);
        }
        else
        {
            Debug.LogError("TableCreator component not found on createTablePanel!");
        }
    }
    #endregion

    #region Player Chair Management
    public void DeactivatePlayerChairs()
    {
        foreach (var chair in playerChairs)
        {
            if (chair != null)
            {
                chair.SetActive(false);
            }
        }
    }

    public void ActivatePlayerChairs(int playerCount)
    {
        if (playerCount <= 0 || playerCount > playerChairs.Count)
        {
            Debug.LogError($"Invalid player count: {playerCount}. Must be between 1 and {playerChairs.Count}");
            return;
        }

        DeactivatePlayerChairs();

        for (int i = 0; i < playerCount; i++)
        {
            if (playerChairs[i] != null)
            {
                playerChairs[i].SetActive(true);
            }
        }
    }
    #endregion

    #region Utility Methods
    public GameObject GetCurrentPanel() => currentActivePanel;

    public bool IsPanelActive(GameObject panel)
    {
        return panel != null && panel.activeSelf;
    }
    #endregion
}
