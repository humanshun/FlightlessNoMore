using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;

public class AddCoinEffect : MonoBehaviour
{
    [SerializeField] private Transform canvas;        // コインを生成するUIの親Transform
    [SerializeField] private GameObject coinPrefab;   // 生成するコインのプレハブ
    [SerializeField] private Transform coinPosition;  // コインが飛び出す起点の位置
    [SerializeField] private Transform addCoinPosition; // コインが集まる最終位置
    [SerializeField] private float spawnRadius = 20.0f; // コインがランダムに散らばる半径

    private List<CoinMover> activeCoins = new List<CoinMover>();
    private CancellationTokenSource spawnCts;

    /// <summary>
    /// コイン飛ばし演出を開始
    /// </summary>
    public void Play(int count)
    {
        // 既存演出をキャンセル
        spawnCts?.Cancel();
        spawnCts = new CancellationTokenSource();

        // 非同期でスポーン開始
        SpawnCoinsAsync(count, spawnCts.Token).Forget();
    }

    private async UniTaskVoid SpawnCoinsAsync(int count, CancellationToken token)
    {
        for (int i = 0; i < count; i++)
        {
            if (token.IsCancellationRequested) break;

            // ランダム位置に生成
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 spawnPos = (Vector2)coinPosition.position + randomOffset;

            GameObject coin = Instantiate(
                coinPrefab,
                spawnPos,
                Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)),
                canvas
            );

            CoinMover mover = coin.AddComponent<CoinMover>();
            mover.Init(addCoinPosition, 1f, OnCoinArrived);
            activeCoins.Add(mover);

            // 次のコインを出すまで50ms待つ（演出感を出す）
            try
            {
                await UniTask.Delay(50, cancellationToken: token);
            }
            catch
            {
                break;
            }
        }
    }

    /// <summary>
    /// 途中でスキップ（飛んでるコインを全て即ゴールに移動）
    /// </summary>
    public void Skip()
    {
        // コイン生成タスクを止める
        spawnCts?.Cancel();

        foreach (var coin in activeCoins.ToArray())
        {
            if (coin != null && !coin.IsCompleted)
                coin.SkipToTarget();
        }
        activeCoins.Clear();
    }

    private void OnCoinArrived(CoinMover coin)
    {
        activeCoins.Remove(coin);
    }
}
