using UnityEngine;

public class RocketVisualizer : MonoBehaviour
{
    [SerializeField] private RocketData rocketData;
    private void OnDrawGizmos()
    {
        if (rocketData == null) return;

        // エフェクトのローカル座標をワールド座標に変換
        Vector3 worldPos = transform.TransformPoint(rocketData.effectPosition);

        // ギズモの色と表示
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(worldPos, 0.2f);
        Gizmos.DrawLine(transform.position, worldPos);
    }
}
