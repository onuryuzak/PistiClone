using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ScoreBoardController : MonoBehaviour
{
    [SerializeField]
    private List<BoardMember> boardMembers = new List<BoardMember>();
    public GameObject boardMemberPrefab;
    public Transform boardMembersParent;
    public GameObject winnerPanel;
    public GameObject loserPanel;

    private void OnEnable()
    {
        EventManager.OnScoreBoard += SetBoard;
    }

    private void OnDisable()
    {
        EventManager.OnScoreBoard -= SetBoard;
    }

    private void SetBoard(List<BaseController> playerControllers)
    {
        if (playerControllers == null || playerControllers.Count == 0)
        {
            Debug.LogError("No player controllers provided to ScoreBoard!");
            return;
        }

        ClearExistingBoardMembers();
        SpawnBoardMembers();

        // Skor tablosunu güncelle
        for (int i = 0; i < playerControllers.Count; i++)
        {
            if (boardMembers[i] != null && playerControllers[i] != null)
            {
                boardMembers[i].SetInfo(i + 1, playerControllers[i].playerType.ToString(), playerControllers[i].point);
            }
        }

        ShowBoard();

        // En yüksek skora sahip oyuncuyu bul
        var highestScorer = playerControllers.OrderByDescending(p => p.point).First();
        var playerController = playerControllers.FirstOrDefault(p => p.playerType == PlayerTypes.Player);

        if (playerController != null)
        {
            bool isPlayerWinner = playerController.point >= highestScorer.point;
            winnerPanel.SetActive(isPlayerWinner);
            loserPanel.SetActive(!isPlayerWinner);
            
            if (isPlayerWinner)
            {
                EventManager.TriggerPlayerWin(true);
                SaveSystem.UpdatePlayerStats(playerController.playerType.ToString(), true, playerController.point, HasPisti(playerController));
            }
            else
            {
                EventManager.TriggerPlayerLose(true);
                SaveSystem.UpdatePlayerStats(playerController.playerType.ToString(), false, playerController.point, HasPisti(playerController));
            }
        }
    }

    private bool HasPisti(BaseController controller)
    {
        if (controller == null || controller.collectedDeckParent == null) return false;
        
        var collector = controller.collectedDeckParent.GetComponent<CardCollector>();
        return collector != null && collector.CollectedCards.Count >= 2;
    }

    private void ClearExistingBoardMembers()
    {
        foreach (var member in boardMembers)
        {
            if (member != null)
            {
                Destroy(member.gameObject);
            }
        }
        boardMembers.Clear();
    }

    public void ShowBoard()
    {
        foreach (var member in boardMembers)
        {
            if (member != null)
            {
                member.gameObject.SetActive(true);
            }
        }
    }

    private void SpawnBoardMembers()
    {
        int playerCount = GameManager.Instance.currentPlayers.Count;
        for (int i = 0; i < playerCount; i++)
        {
            var go = Instantiate(boardMemberPrefab, boardMembersParent);
            var boardMember = go.GetComponent<BoardMember>();
            if (boardMember != null)
            {
                boardMembers.Add(boardMember);
            }
        }
    }
}
