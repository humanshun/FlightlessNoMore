using UnityEngine;

public class Moon : MonoBehaviour
{
    public Transform player;
    void OnEnable()
    {
        // GameManagerのイベントに、自分の「OnPlayerSpawned」メソッドを登録
        GameManager.OnInGamePlayerSpawned += OnPlayerSpawned;
    }
    void OnDisable()
    {
        GameManager.OnInGamePlayerSpawned -= OnPlayerSpawned;
    }
    private void OnPlayerSpawned(CustomPlayer spawnedPlayer)
    {
        // GameManagerから渡されたプレイヤーのTransformをPlayerに保存
        player = spawnedPlayer.transform;
    }
}
