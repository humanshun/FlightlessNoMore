using System.Collections.Generic;
using UnityEngine;

// ScriptableObjectとして作成可能にする
[CreateAssetMenu(fileName = "New Body", menuName = "Parts/Body")]
public class BodyData : PartData
{
    [Header("空気抵抗")]
    [Tooltip("値が高いほど空気抵抗が大きくなります。")]
    public AirResistance airResistance;  // 抗力のプロパティ

    [Header("体力")]
    [Tooltip("値が高いほど体力が大きくなります。")]
    public Health hp;  // 体力

    [Header("ロケットの位置")]
    [Tooltip("ロケットの位置を指定します。")]
    public Vector2 rocketPosition;  // ロケットの位置

    [Header("ウィングの位置")]
    [Tooltip("ウィングの位置を指定します。")]
    public Vector2 wingPosition;    // ウィングの位置

    [Header("車輪の位置")]
    [Tooltip("車輪の位置を指定します。")]
    public Vector2 rightTirePosition;  // 車輪の位置
    public Vector2 leftTirePosition;   // 車輪の位置

    [System.Serializable]
    public class AirResistance
    {
        [HideInInspector] public string displayName = "空気抵抗";
        public float value;
    }

    [System.Serializable]
    public class Health
    {
        [HideInInspector] public string displayName = "体力";
        public float value;
    }
}
