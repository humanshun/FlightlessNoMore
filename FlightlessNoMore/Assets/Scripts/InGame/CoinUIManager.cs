using UnityEngine;
using Cysharp.Threading.Tasks;

public enum CoinUIState
{
    Idle,      // 何もしていない
    Count,     // コイン演出（飛ばす & 数字アニメーション）
    Skipped    // 演出スキップして最終値表示
}

public class CoinUIManager : MonoBehaviour
{
    [SerializeField] private AddCoinEffect addCoinEffect;  // コインPrefab演出
    [SerializeField] private CoinDisplay coinDisplay;      // 数字アニメーション

    private CoinUIState state = CoinUIState.Idle;          // 現在の状態
    private int currentEarnedCoins = 0;                    // 今回の獲得コイン数を保持

    /// コイン演出をまとめて開始する
    public void PlayCoinUI(int earnedCoins)
    {
        if (state != CoinUIState.Idle)
        {
            Debug.Log("CoinUIManager: Idleじゃないので再生できない");
            return;
        }

        Debug.Log("CoinUIManager: コイン演出開始");
        state = CoinUIState.Count;

        currentEarnedCoins = earnedCoins; // スキップ用に保持

        // Prefab演出
        addCoinEffect.Play(earnedCoins);

        // 数字アニメーション（終わったらFinishCoinUI呼ぶ）
        coinDisplay.PlayCountUpAsync(earnedCoins, FinishCoinUI).Forget();
    }

    /// 演出スキップ（Prefabも数字も即終了）
    public void SkipCoinUI()
    {
        if (state != CoinUIState.Count)
        {
            //演出中じゃないのでスキップ不要
            return;
        }

        //演出スキップ
        //AddCoinEffectはSkip
        addCoinEffect.Skip();

        //CoinDisplayはSkip(earnedCoins)を呼ぶ
        coinDisplay.Skip(currentEarnedCoins);

        state = CoinUIState.Skipped;
        FinishCoinUI();
    }

    /// 演出終了時（スキップ or 正常完了後）
    private void FinishCoinUI()
    {
        Debug.Log("CoinUIManager: コイン演出終了 → Idleに戻す");
        state = CoinUIState.Idle;
    }

    /// 今演出中かどうか
    public bool IsPlaying => state == CoinUIState.Count;
    void Update()
    {
        if (IsPlaying && Input.GetMouseButtonDown(0))
        {
            SkipCoinUI();
        }
    }
}
