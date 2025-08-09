using System;
using System.Collections.Generic;
using UnityEngine;
public class PartData : ScriptableObject
{
    [Header("パーツ名")]
    public string partName; // 表示用：日本語おｋ

    [Header("ファイル名")]
    public string resaurceFileName; //読み込み用：英語のファイル名で一致させる

    [Header("プレハブ")]
    public GameObject partPrefab; // パーツのプレハブ

    [Header("重量")]
    [Tooltip("値が高いほど重力が大きくなります。")]
    public PartWeight weight;    // 重量（すべてのパーツに共通）

    [Header("金額")]
    [Tooltip("このパーツの価格")]
    public PartCost partCost;

    [Header("タイプ")]
    [Tooltip("このパーツの種類")]
    public PartType partType; // パーツの種類

    [Header("説明")]
    [Tooltip("このパーツの説明")]
    public string partDescription; // パーツの説明

    [Header("アイコン")]
    [Tooltip("このパーツのアイコン")]
    public Sprite partIconImage; // パーツのアイコン画像

    [System.Serializable]
    public class PartCost
    {
        [HideInInspector]public string displayName = "金額";
        public int value;
    }

    [System.Serializable]
    public class PartWeight
    {
        [HideInInspector]public string displayName = "重量";
        public float value;
    }
}