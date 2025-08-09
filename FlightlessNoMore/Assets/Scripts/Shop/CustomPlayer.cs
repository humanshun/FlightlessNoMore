using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomPlayer : MonoBehaviour
{
    [SerializeField] private Transform contentTransform;

    void Start()
    {
        SetupAll();
    }
    public void SetupAll()
    {
        // すでに子オブジェクトがある場合、全て削除してリセット
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        // 現在装備中のパーツデータを取得（PartType -> string（Resourcesの名前））
        var currentPart = PlayerData.Instance.GetAllCurrentParts();

        //bodyから読み込む（位置情報を取得するため）
        if (!currentPart.TryGetValue(PartType.Body, out var bodyResourceName))
        {
            Debug.LogWarning("Bodyパーツが装備されていません。");
            return;
        }

        // Bodyデータ（プレハブ付きのScriptableObject）をResourcesからロード
        BodyData body = Resources.Load<BodyData>($"Parts/{bodyResourceName}");
        if (body == null)
        {
            Debug.LogWarning($"Body{bodyResourceName} が見つかりませんでした。");
            return;
        }

        // Bodyパーツのプレハブを生成して子として配置
        GameObject bodyInstance = Instantiate(body.partPrefab, contentTransform);

        // 見た目上の「底（地面）」の位置を取得し、位置調整
        Vector3 bottomOffset = CalculateBottomoffset(bodyInstance);
        bodyInstance.transform.localPosition = -bottomOffset;

        // 各種パーツ（Rocket, Wing, Tire）をBodyの位置情報に従ってセットアップ
        SetupPart(currentPart, PartType.Rocket, body.rocketPosition);
        SetupPart(currentPart, PartType.Wing, body.wingPosition);
        SetupPart(currentPart, PartType.Tire, body.rightTirePosition);
        SetupPart(currentPart, PartType.Tire, body.leftTirePosition);
    }

    // 指定されたパーツを指定位置に生成する（Rocket, Wing, Tireなど）
    private void SetupPart(Dictionary<PartType, string> currentPart, PartType partType, Vector2 localPos)
    {
        // 指定されたタイプのパーツが存在するか確認
        if (!currentPart.TryGetValue(partType, out var partResourceName)) return;

        // Resources から該当のパーツデータをロード
        PartData part = Resources.Load<PartData>($"Parts/{partResourceName}");
        if (part == null)
        {
            Debug.LogWarning($"Part{partResourceName} が見つかりませんでした。");
            return;
        }

        // パーツを生成し、指定された位置にローカル配置
        var instance = Instantiate(part.partPrefab, contentTransform);
        instance.transform.localPosition = localPos;
    }

    // 与えられたオブジェクトの「一番下のY座標」（地面との接地位置）を計算する
    private Vector3 CalculateBottomoffset(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("Rendererが見つかりません（Body補正できません）");
            return Vector3.zero;
        }

        // Y座標の最小値を取得（地面に近い位置）
        float minY = float.MaxValue;
        foreach (var r in renderers)
        {
            // ワールド座標からローカル座標に変換して最小値を調べる
            float localY = obj.transform.InverseTransformPoint(r.bounds.min).y;
            if (localY < minY)
            {
                minY = localY;
            }
        }

        // Y座標だけ下方向に補正をかける
        return new Vector3(0f, minY, 0f);
    }
}
