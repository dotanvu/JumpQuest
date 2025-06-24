using UnityEngine;
using Unity.Cinemachine;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject playerInstance;

    void Start()
    {
        // Kiểm tra xem đã có player trong scene chưa
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null)
        {
            // Nếu đã có player, không tạo thêm
            playerInstance = existingPlayer;
        }
        else
        {
            // Tạo player mới
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            Vector3 spawnPosition = transform.position;

            if (gameManager != null && gameManager.hasCheckpoint)
            {
                spawnPosition = gameManager.checkpointPosition;
            }

            playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        }

        // Cập nhật tham chiếu trong GameManager
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null && playerInstance != null)
        {
            gm.player = playerInstance.transform;
        }

        // Cập nhật camera
        CinemachineCamera virtualCam = FindFirstObjectByType<CinemachineCamera>();
        if (virtualCam != null && playerInstance != null)
        {
            virtualCam.Follow = playerInstance.transform;
        }
    }
}