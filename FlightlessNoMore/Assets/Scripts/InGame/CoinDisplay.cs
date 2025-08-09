using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class CoinDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private float interval = 0.08f;
    [SerializeField] private float startDelay = 1.55f;

    private int currentCoin = 0;

    // 内部状態は外から見なくてOK、外部でステート管理する
    private bool skipRequested = false;

    void Start()
    {
        // 初期化（プレイヤーデータの所持数を表示）
        currentCoin = PlayerData.Instance.playerCoins;
        coinText.text = currentCoin.ToString();
    }

    /// <summary>
    /// 指定枚数のコインを1枚ずつ加算する演出
    /// </summary>
    public async UniTask PlayCountUpAsync(int earnedCoins, Action onComplete = null)
    {
        skipRequested = false;

        int targetCoin = currentCoin + earnedCoins;

        // コインが飛んでくる演出と合わせるための遅延
        await UniTask.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken: this.GetCancellationTokenOnDestroy());

        while (currentCoin < targetCoin)
        {
            if (skipRequested)
            {
                // スキップ要求が来たら即最終値に
                currentCoin = targetCoin;
                coinText.text = currentCoin.ToString();
                PlayerData.Instance.playerCoins = currentCoin;
                onComplete?.Invoke();
                return;
            }

            // 1枚増やす
            currentCoin++;
            coinText.text = currentCoin.ToString();

            await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        // 最終値をセーブ
        PlayerData.Instance.playerCoins = currentCoin;

        // 終わったらコールバック通知
        onComplete?.Invoke();
    }

    /// <summary>
    /// 外部から演出をスキップ（即最終値にする）
    /// </summary>
    public void Skip(int earnedCoins)
    {
        skipRequested = true;

        int targetCoin = currentCoin + earnedCoins;
        currentCoin = targetCoin;
        coinText.text = currentCoin.ToString();

        // プレイヤーデータに反映
        PlayerData.Instance.playerCoins = currentCoin;
    }

    /// <summary>
    /// 即座にコインを増やす（アニメーションなし）
    /// </summary>
    public void AddCoinsImmediately(int amount)
    {
        currentCoin += amount;
        coinText.text = currentCoin.ToString();
        PlayerData.Instance.playerCoins = currentCoin;
    }
}
