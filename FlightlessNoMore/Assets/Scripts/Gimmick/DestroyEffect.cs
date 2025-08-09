using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    private ParticleSystem ps;
    private float delayTime; // 遅延時間

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();

        if (ps == null)
        {
            Debug.LogError("ParticleSystemが見つかりません");
            Destroy(gameObject); // ParticleSystemがなければ即座に削除
        }
        else
        {
            // StartLifetimeの最大値を遅延時間として取得
            var main = ps.main;
            delayTime = main.startLifetime.constantMax; // StartLifetimeがランダム範囲の場合も考慮
        }
    }

    private void Update()
    {
        // ParticleSystemが停止している場合、遅延して削除
        if (ps != null && !ps.IsAlive())
        {
            StartCoroutine(DestroyWithDelay());
        }
    }

    private System.Collections.IEnumerator DestroyWithDelay()
    {
        // 遅延時間分待機
        yield return new WaitForSeconds(delayTime);
        Destroy(gameObject); // エフェクトを削除
    }
}
