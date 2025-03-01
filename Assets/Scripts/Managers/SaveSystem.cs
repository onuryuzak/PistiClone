using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public string playerName;
    public int totalScore;
    public int gamesPlayed;
    public int gamesWon;
    public float winRate;
    public int pistiCount;
    public List<int> lastScores;

    public PlayerData(string name)
    {
        playerName = name;
        totalScore = 0;
        gamesPlayed = 0;
        gamesWon = 0;
        winRate = 0f;
        pistiCount = 0;
        lastScores = new List<int>();
    }

    public void UpdateStats(bool isWinner, int score, bool hasPisti)
    {
        gamesPlayed++;
        if (isWinner) gamesWon++;
        winRate = (float)gamesWon / gamesPlayed;
        totalScore += score;
        if (hasPisti) pistiCount++;
        
        // Son 5 skoru tut
        lastScores.Add(score);
        if (lastScores.Count > 5)
        {
            lastScores.RemoveAt(0);
        }
    }
}



public class SaveSystem : MonoBehaviour
{
    private static string PLAYER_DATA_PATH => Path.Combine(Application.persistentDataPath, "playerData.json");

    #region Player Data Operations
    public static void SavePlayerData(PlayerData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(PLAYER_DATA_PATH, json);
            Debug.Log($"Player data saved successfully for: {data.playerName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save player data: {e.Message}");
        }
    }

    public static PlayerData LoadPlayerData(string playerName)
    {
        try
        {
            if (DoesSaveExist())
            {
                string json = File.ReadAllText(PLAYER_DATA_PATH);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                return data;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load player data: {e.Message}");
        }

        return new PlayerData(playerName);
    }

    public static void UpdatePlayerStats(string playerName, bool isWinner, int score, bool hasPisti)
    {
        PlayerData data = LoadPlayerData(playerName);
        data.UpdateStats(isWinner, score, hasPisti);
        SavePlayerData(data);
    }
    #endregion



    #region File Management
    public static void DeleteAllSaveData()
    {
        try
        {
            if (DoesSaveExist())
                File.Delete(PLAYER_DATA_PATH);
            Debug.Log("All save data deleted successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete save data: {e.Message}");
        }
    }

    public static bool DoesSaveExist()
    {
        return File.Exists(PLAYER_DATA_PATH);
    }
    #endregion
} 