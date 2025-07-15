using UnityEngine;

public class CheckpointPlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                if (gameObject.CompareTag("MovingPlatform"))
                {
                    GameObject[] respawnPoints = GameObject.FindGameObjectsWithTag("RespawnPoint");
                    if (respawnPoints.Length > 0)
                    {
                        GameObject nearestRespawnPoint = FindNearestRespawnPoint(respawnPoints, gameObject.transform.position);
                        if (nearestRespawnPoint != null)
                        {
                            gameManager.SetCheckpoint(nearestRespawnPoint.transform.position);
                        }
                    }
                }
                else if (gameObject.CompareTag("Checkpoint"))
                {
                    gameManager.SetCheckpoint(transform.position);

                    // Auto-save khi đạt checkpoint
                    gameManager.SaveGame();
                }
            }
        }
    }

    private GameObject FindNearestRespawnPoint(GameObject[] respawnPoints, Vector3 currentPosition)
    {
        GameObject nearest = null;
        float minDistance = float.MaxValue;
        foreach (GameObject respawn in respawnPoints)
        {
            if (respawn != null)
            {
                float distance = Vector3.Distance(respawn.transform.position, currentPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = respawn;
                }
            }
        }
        return nearest;
    }
}