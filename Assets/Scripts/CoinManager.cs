using UnityEngine;

public class CoinManager : MonoBehaviour
{
    void Start()
    {
        // Đợi 1 frame để đảm bảo session ID đã được cập nhật
        StartCoroutine(CheckCoinsAfterDelay());
    }

    private System.Collections.IEnumerator CheckCoinsAfterDelay()
    {
        yield return null; // Đợi 1 frame

        // Tìm tất cả coin trong scene và kiểm tra trạng thái
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");

        Debug.Log("CoinManager: Found " + coins.Length + " coins");

        // Test: Tạo coin mới để kiểm tra
        if (coins.Length == 0)
        {
            Debug.LogWarning("No coins found! Creating test coin...");
            CreateTestCoin();
        }

        foreach (GameObject coin in coins)
        {
            // Force hiện coin với mọi cách
            coin.SetActive(true);
            coin.transform.localScale = Vector3.one; // Đảm bảo scale = 1

            // Kiểm tra SpriteRenderer
            SpriteRenderer sr = coin.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(1, 1, 1, 1); // Đảm bảo alpha = 1
            }

            // Tạo ID đơn giản dựa trên vị trí
            string coinID = "Coin_" + coin.transform.position.x.ToString("F1") + "_" + coin.transform.position.y.ToString("F1");

            // Nếu coin đã được thu thập thì ẩn nó
            if (PlayerPrefs.GetInt(coinID, 0) == 1)
            {
                coin.SetActive(false);
                Debug.Log("Hiding coin: " + coinID + " at " + coin.transform.position);
            }
            else
            {
                Debug.Log("Showing coin: " + coinID + " at " + coin.transform.position + " - Active: " + coin.activeInHierarchy);
            }
        }
    }

    // Tạo coin test để kiểm tra
    private void CreateTestCoin()
    {
        GameObject testCoin = new GameObject("TestCoin");
        testCoin.tag = "Coin";
        testCoin.transform.position = new Vector3(0, 2, 0); // Vị trí dễ thấy

        // Thêm SpriteRenderer
        SpriteRenderer sr = testCoin.AddComponent<SpriteRenderer>();
        sr.color = Color.yellow; // Màu vàng dễ thấy
        sr.sprite = Resources.Load<Sprite>("DefaultSprite"); // Hoặc tạo sprite đơn giản

        // Thêm Collider
        CircleCollider2D col = testCoin.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        Debug.Log("Test coin created at " + testCoin.transform.position);
    }
}
