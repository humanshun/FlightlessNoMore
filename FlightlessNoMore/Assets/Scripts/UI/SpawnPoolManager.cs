using UnityEngine;
using System.Collections.Generic;

// Prefabごとに生成数を指定
[System.Serializable]
public class SpawnablePrefab
{
    public GameObject prefab;
    public int instanceCount = 10; // 何個生成するか指定
}

public class SpawnPoolManager : MonoBehaviour
{
    public Transform playerPosition;
    public SpawnablePrefab[] spawnPrefabs;

    [Header("表示設定")]
    public float spawnRadius = 1000f;       // プレイヤーを中心にランダムスポーンする範囲
    public float reSpawnDistance = 500f;    // 半径外に出たオブジェクトの再スポーン距離

    [Header("スポーン間隔設定")]
    public float minDistanceBetweenObjects = 50f; // 既存オブジェクトとの最小距離

    [Header("プレイヤーとの距離制御")]
    public float minDistanceFromPlayer = 100f;    // プレイヤーから最低でもこれ以上離す

    [Header("スポーン方向設定")]
    public bool spawnOnlyInFront = true;          // trueならプレイヤー前方のみスポーン
    public float spawnFrontAngle = 90f;           // 前方の角度(°)

    private List<GameObject> activeObjects = new List<GameObject>();
    private Queue<GameObject> prefabPool = new Queue<GameObject>();

    private bool initialized = false;

    private const float minYThreshold = -10f; // これより下には生成しない

    void OnEnable()
    {
        GameManager.OnInGamePlayerSpawned += OnPlayerSpawned;
    }

    void OnDisable()
    {
        GameManager.OnInGamePlayerSpawned -= OnPlayerSpawned;
    }

    void Start()
    {
        // Prefabごとに指定された個数を生成
        foreach (var p in spawnPrefabs)
        {
            for (int i = 0; i < p.instanceCount; i++)
            {
                GameObject obj = Instantiate(p.prefab);
                obj.SetActive(false);
                prefabPool.Enqueue(obj);
            }
        }
    }

    void Update()
    {
        if (playerPosition == null || !initialized) return;

        int maxSpawnAttempts = 100; // 無限ループ防止
        int spawnAttempts = 0;

        // プールに余りがあればスポーンする
        while (prefabPool.Count > 0 && spawnAttempts < maxSpawnAttempts)
        {
            SpawnInitialCloud();
            spawnAttempts++;
        }

        // 半径外に出たものはプールに戻して、新しい場所にリスポーン
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = activeObjects[i];
            if (obj == null) // 破棄されたオブジェクト対策
            {
                activeObjects.RemoveAt(i);
                continue;
            }

            float distance = Vector2.Distance(obj.transform.position, playerPosition.position);

            if (distance > spawnRadius)
            {
                obj.SetActive(false);
                prefabPool.Enqueue(obj);
                activeObjects.RemoveAt(i);

                // プールに戻したら、新しい位置で再配置
                SpawnCloudOutsideRadius();
            }
        }
    }

    void SpawnInitialCloud()
    {
        if (prefabPool.Count == 0) return;

        GameObject obj = prefabPool.Dequeue();

        Vector2 offset;
        float y;
        int attempts = 0;
        bool found = false;

        do
        {
            // プレイヤーの周囲からランダムに選ぶ
            offset = Random.insideUnitCircle * spawnRadius;

            // プレイヤーの進行方向の前方だけに絞る場合
            if (spawnOnlyInFront)
            {
                Vector2 forward = GetPlayerForward();
                float angleToCandidate = Vector2.Angle(forward, offset);
                if (angleToCandidate > spawnFrontAngle * 0.5f)
                {
                    attempts++;
                    continue; // 前方角度外ならやり直し
                }
            }

            y = playerPosition.position.y + offset.y;
            Vector2 candidatePos = new Vector2(playerPosition.position.x + offset.x, y);

            // Y座標が最低値以上 & 既存オブジェクト/プレイヤーから十分離れていれば採用
            if (y >= minYThreshold && IsPositionValid(candidatePos))
            {
                obj.transform.position = new Vector3(candidatePos.x, candidatePos.y, 0f);
                obj.SetActive(true);
                activeObjects.Add(obj);
                found = true;
                break;
            }

            attempts++;
        } while (!found && attempts < 20);

        if (!found)
        {
            prefabPool.Enqueue(obj); // 見つからなかったら戻す
        }
    }

    void SpawnCloudOutsideRadius()
    {
        if (prefabPool.Count == 0) return;

        GameObject obj = prefabPool.Dequeue();

        Vector2 playerDir = GetPlayerForward();

        Vector2 offset;
        float y;
        int attempts = 0;
        bool found = false;

        do
        {
            // プレイヤー進行方向を中心に±spawnFrontAngle/2の範囲にスポーン
            float baseAngle = Mathf.Atan2(playerDir.y, playerDir.x);
            float randomAngle = baseAngle + Random.Range(-Mathf.Deg2Rad * spawnFrontAngle / 2, Mathf.Deg2Rad * spawnFrontAngle / 2);

            float distance = Random.Range(spawnRadius + 50f, reSpawnDistance);
            offset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * distance;

            y = playerPosition.position.y + offset.y;
            Vector2 candidatePos = new Vector2(playerPosition.position.x + offset.x, y);

            if (y >= minYThreshold && IsPositionValid(candidatePos))
            {
                obj.transform.position = new Vector3(candidatePos.x, candidatePos.y, 0f);
                obj.SetActive(true);
                activeObjects.Add(obj);
                found = true;
                break;
            }

            attempts++;
        } while (!found && attempts < 20);

        if (!found)
        {
            prefabPool.Enqueue(obj); // 見つからなければ戻す
        }
    }

    private void OnPlayerSpawned(CustomPlayer spawnedPlayer)
    {
        playerPosition = spawnedPlayer.transform;
        initialized = true;

        // プレイヤーがスポーンしたら、プールにあるだけ全部初期スポーン
        int spawnAttempts = 0;
        int maxSpawnAttempts = prefabPool.Count;

        while (prefabPool.Count > 0 && spawnAttempts < maxSpawnAttempts)
        {
            SpawnInitialCloud();
            spawnAttempts++;
        }
    }

    private bool IsPositionValid(Vector2 candidatePos)
    {
        // プレイヤーとの距離チェック
        if (playerPosition != null)
        {
            float playerDist = Vector2.Distance(candidatePos, playerPosition.position);
            if (playerDist < minDistanceFromPlayer)
            {
                return false; // プレイヤーに近すぎるので無効
            }
        }

        // 既存オブジェクトとの距離チェック
        foreach (var obj in activeObjects)
        {
            if (obj == null) continue;

            float dist = Vector2.Distance(candidatePos, obj.transform.position);
            if (dist < minDistanceBetweenObjects)
            {
                return false; // 近すぎるので無効
            }
        }
        return true; // 全部OK
    }

    // プレイヤーの進行方向ベクトルを取得
    private Vector2 GetPlayerForward()
    {
        Vector2 forward = Vector2.right; // デフォルトは右向き
        if (playerPosition.TryGetComponent<Rigidbody2D>(out var rb))
        {
            if (rb.linearVelocity.sqrMagnitude > 0.1f)
                forward = rb.linearVelocity.normalized;
        }
        return forward;
    }
}
