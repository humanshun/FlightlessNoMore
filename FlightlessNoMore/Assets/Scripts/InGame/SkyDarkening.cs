using UnityEngine;
using System.Collections;

public class SkyDarkening : MonoBehaviour
{
    public Camera mainCamera; // カメラを指定
    public Transform player; // プレイヤーの位置を取得
    public float minHeight = 0f; // 最小高さ
    public float maxHeight = 100f; // 最大高さ
    public Color startColor = Color.cyan; // 開始時の空の色
    public Color endColor = Color.black; // 終了時の空の色
    public Vector3 playerPosition; // プレイヤーの位置を保存

    // このスクリプトが有効になった時に呼ばれる（シーン開始やオブジェクトが有効化された時）
    void OnEnable()
    {
        // GameManagerのイベントに、自分の「OnPlayerSpawned」メソッドを登録
        GameManager.OnInGamePlayerSpawned += OnPlayerSpawned;
    }
    void OnDisable()
    {
        GameManager.OnInGamePlayerSpawned -= OnPlayerSpawned;
    }

    // GameManagerがInGamePlayerを生成したときに呼ばれる処理
    private void OnPlayerSpawned(CustomPlayer spawnedPlayer)
    {
        // GameManagerから渡されたプレイヤーのTransformをPlayerに保存
        player = spawnedPlayer.transform;
    }

    void Update()
    {
        if (player == null) return;
        // プレイヤーの高さを取得
        float playerHeight = Mathf.Clamp(player.position.y, minHeight, maxHeight);

        // 高さに応じて色を補間
        float t = (playerHeight - minHeight) / (maxHeight - minHeight);
        mainCamera.backgroundColor = Color.Lerp(startColor, endColor, t);

        playerPosition = player.position;
    }
}
