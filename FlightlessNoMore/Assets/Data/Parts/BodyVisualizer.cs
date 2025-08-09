using UnityEngine;

[ExecuteAlways] // エディタでも実行される
public class BodyVisualizer : MonoBehaviour
{
    public BodyData bodyData;
    [SerializeField] private Vector2 offset = Vector2.zero; // オフセットを追加

    private void OnDrawGizmos()
    {
        if (bodyData == null) return;

        // 各座標のオフセットをローカル → ワールド変換
        DrawPoint(bodyData.rocketPosition, Color.red, "Rocket");
        DrawPoint(bodyData.wingPosition, Color.green, "Wing");
        DrawPoint(bodyData.rightTirePosition, Color.cyan, "Tire R");
        DrawPoint(bodyData.leftTirePosition, Color.cyan, "Tire L");
    }

    private void DrawPoint(Vector2 localPos, Color color, string label)
    {
        Vector3 worldPos = transform.TransformPoint(localPos + offset); // ←ここを修正

        Gizmos.color = color;
        Gizmos.DrawSphere(worldPos, 0.15f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(worldPos + Vector3.up * 0.2f, label);
#endif
    }
}
