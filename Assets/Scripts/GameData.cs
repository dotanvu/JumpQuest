using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public int playerHealth;
    public int currentScore;
    public string currentScene;
    public Vector3 checkpointPosition;
    public bool hasCheckpoint;
    public List<string> enemyKilledList;

    public GameData()
    {
        playerPosition = Vector3.zero;
        playerHealth = 3;
        currentScore = 0;
        currentScene = "Game";
        checkpointPosition = Vector3.zero;
        hasCheckpoint = false;
        enemyKilledList = new List<string>();
    }

    public GameData(Vector3 playerPos, int health, int score, string scene, Vector3 checkpoint, bool hasCP, List<string> killedEnemyList = null)
    {
        playerPosition = playerPos;
        playerHealth = health;
        currentScore = score;
        currentScene = scene;
        checkpointPosition = checkpoint;
        hasCheckpoint = hasCP;
        enemyKilledList = killedEnemyList ?? new List<string>();
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(currentScene) && playerHealth >= 0 && currentScore >= 0;
    }
}
