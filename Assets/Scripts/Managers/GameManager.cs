using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private TableType currentTableType = TableType.Newbies;

    [HideInInspector] public GameStatus gameStatus;

    public GameLogic GameLogic;
    public TextMeshProUGUI betText;

    [HideInInspector] public int currentBet;

    public List<BaseController> FourPlayers;
    public List<BaseController> TwoPlayers;
    [HideInInspector] public List<BaseController> currentPlayers;
    [HideInInspector] public int currentWinCount;
    [HideInInspector] public int currentLoseCount;

    [Header("AI Settings")] 
    [SerializeField] private AIDifficultyData easyAI;
    [SerializeField] private AIDifficultyData mediumAI;
    [SerializeField] private AIDifficultyData hardAI;

    protected override void Awake()
    {
        base.Awake();
        // Load AI difficulty presets if not assigned
        if (easyAI == null) easyAI = Resources.Load<AIDifficultyData>("AI/EasyAI");
        if (mediumAI == null) mediumAI = Resources.Load<AIDifficultyData>("AI/MediumAI");
        if (hardAI == null) hardAI = Resources.Load<AIDifficultyData>("AI/HardAI");
    }

    private void OnEnable()
    {
        EventManager.OnGameStart += GameStartPanels;
        EventManager.OnRoundEnd += () => ChangeGameStatus(GameStatus.WaitForPlay);
        EventManager.OnRoundReady += () => ChangeGameStatus(GameStatus.Play);
        EventManager.OnGameEnd += EndGame;
        EventManager.OnPlayerWin += PlayerWinLose;
        EventManager.OnPlayerLose += PlayerWinLose;
        EventManager.OnGameStatusChanged += StopAllPlayers;
    }

    private void OnDisable()
    {
        EventManager.OnGameStart -= GameStartPanels;
        EventManager.OnRoundEnd -= () => ChangeGameStatus(GameStatus.WaitForPlay);
        EventManager.OnRoundReady -= () => ChangeGameStatus(GameStatus.Play);
        EventManager.OnGameEnd -= EndGame;
        EventManager.OnPlayerWin -= PlayerWinLose;
        EventManager.OnPlayerLose -= PlayerWinLose;
        EventManager.OnGameStatusChanged -= StopAllPlayers;
    }

    private void GameStartPanels(int playerNumber, int bet)
    {
        ChangeGameStatus(GameStatus.WaitForPlay);
        currentBet = bet;
        betText.text = "BET: " + currentBet;
        UIManager.Instance.HandleGameStateChange(gameStatus);

        Invoke("CardManagerInitalize", 0.5f);

        // Player list validation
        if (TwoPlayers == null || TwoPlayers.Count == 0)
        {
            Debug.LogError("TwoPlayers list is not initialized in GameManager!");
            return;
        }

        if (FourPlayers == null || FourPlayers.Count == 0)
        {
            Debug.LogError("FourPlayers list is not initialized in GameManager!");
            return;
        }

        if (playerNumber == 2)
        {
            currentPlayers = new List<BaseController>(TwoPlayers);
            Debug.Log($"Initializing 2 player game. Player count: {currentPlayers.Count}");
            SetupAIPlayers(1);
        }
        else
        {
            currentPlayers = new List<BaseController>(FourPlayers);
            Debug.Log($"Initializing 4 player game. Player count: {currentPlayers.Count}");
            SetupAIPlayers(3);
        }

        // Validate current players
        for (int i = 0; i < currentPlayers.Count; i++)
        {
            if (currentPlayers[i] == null)
            {
                Debug.LogError($"Player at index {i} is null in currentPlayers list!");
                return;
            }
        }

        UIManager.Instance.ActivatePlayerChairs(currentPlayers.Count);
        if (GameLogic == null)
        {
            Debug.LogError("GameLogic reference is not set in GameManager!");
            return;
        }

        GameLogic.Initialize(playerNumber, currentPlayers);
    }

    private void ChangeGameStatus(GameStatus newStatus)
    {
        gameStatus = newStatus;
        EventManager.TriggerGameStatusChanged();
    }

    private void StopAllPlayers()
    {
        if (gameStatus != GameStatus.WaitForPlay)
            return;

        for (int i = 0; i < currentPlayers.Count; i++)
        {
            currentPlayers[i].canPlay = false;
        }
    }

    private void CardManagerInitalize()
    {
        CardManager.Instance.InitializeDeck();
    }

    public void EndGame()
    {
        ChangeGameStatus(GameStatus.ScoreBoard);
        UIManager.Instance.HandleGameStateChange(gameStatus);
        EventManager.TriggerScoreBoard(SortPlayersByPoint());
    }

    public List<BaseController> SortPlayersByPoint()
    {
        return currentPlayers.OrderByDescending(player => player.point).ToList();
    }

    private void PlayerWinLose(bool isWin)
    {
        if (isWin)
        {
            PlayerPrefs.SetInt(PrefsKeys.WinCount, PlayerPrefs.GetInt(PrefsKeys.WinCount, 0) + 1);
            ExchangeManager.Instance.AddCurrency(CurrencyType.Cash, currentBet * currentPlayers.Count);
        }
        else
        {
            PlayerPrefs.SetInt(PrefsKeys.LoseCount, PlayerPrefs.GetInt(PrefsKeys.LoseCount, 0) + 1);
        }

        currentWinCount = PlayerPrefs.GetInt(PrefsKeys.WinCount, 0);
        currentLoseCount = PlayerPrefs.GetInt(PrefsKeys.LoseCount, 0);
    }

    public void RestartGame()
    {
        ChangeGameStatus(GameStatus.Menu);
        UIManager.Instance.HandleGameStateChange(gameStatus);
    }

    private void SetupAIPlayers(int aiCount)
    {
        // Get AI difficulty based on table type
        AIDifficultyData aiDifficulty = GetAIDifficultyForTableType(currentTableType);

        // Find all AI controllers and set their difficulty
        for (int i = 1; i <= aiCount; i++)
        {
            var aiController = currentPlayers.Find(p => p.playerType == (PlayerTypes)i);
            if (aiController != null && aiController.TryGetComponent<AIControl>(out var aiComp))
            {
                aiComp.SetDifficulty(aiDifficulty);
            }
        }
    }

    private AIDifficultyData GetAIDifficultyForTableType(TableType tableType)
    {
        switch (tableType)
        {
            case TableType.Newbies:
                return easyAI;
            case TableType.Rookies:
                return mediumAI;
            case TableType.Nobles:
                return hardAI;
            default:
                Debug.LogWarning($"Unknown table type: {tableType}, defaulting to easy AI");
                return easyAI;
        }
    }

    public void SetTableType(TableType tableType)
    {
        currentTableType = tableType;
    }
}