using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

// プレイヤーのデータを管理するシングルトンクラス
public class PlayerData : MonoBehaviour
{
    // シングルトンインスタンス
    public static PlayerData Instance;

    // プレイヤーの所持コイン数
    public int playerCoins;

    // コインが変化したときに発火するイベント
    public static event System.Action<int> OnCoinsChanged;

    //チュートリアル進行イベント
    public static event System.Action OnPartPurchased;
    public static event System.Action OnEquippedNonInitialPart;

    // 購入済みパーツのリスト（パーツ名で管理）
    private List<string> purchasedParts = new List<string>();

    // 現在装備しているパーツの名前
    private Dictionary<PartType, string> currentParts = new();

    public float MaxDistance { get; private set; }
    public float MaxAltitude { get; private set; }

    // セーブファイルの保存先パス
    private string SavePath => Path.Combine(Application.persistentDataPath, "PlayerData_save.json");

    public static event System.Action OnAnyPartEquipped;

    void Awake()
    {
        // シングルトンパターン：すでに存在していれば自分を破棄、いなければ自分をInstanceとして残す
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されないようにする
            LoadPlayerData(); // 起動時に保存データを読み込む
        }
        else
        {
            Destroy(gameObject); // すでに存在しているなら自分を削除
        }
    }

    // パーツ購入を試みる処理（成功ならtrue）
    public bool TryBuyPart(PartData part, int price)
    {
        if (playerCoins >= price && !purchasedParts.Contains(part.partName))
        {
            playerCoins -= price; // コインを減らす
            purchasedParts.Add(part.partName); // 購入済みリストに追加（重複チェックなし）
            SavePlayerData(); // データを保存
            Debug.Log("購入成功");

            // イベントを発火
            OnCoinsChanged?.Invoke(playerCoins);
            OnPartPurchased?.Invoke();

            return true;
        }
        else
        {
            Debug.Log("購入失敗");
            return false;
        }
    }

    // 購入済みリストに追加する（重複を防ぐ）
    public void SavePurchasedPart(PartData part)
    {
        if (!purchasedParts.Contains(part.partName))
        {
            purchasedParts.Add(part.partName);
            SavePlayerData(); // 保存を反映
        }
    }

    // 現在装備しているパーツを保存（パーツ名だけ記録）
    public void SaveCurrentPart(PartData part)
    {
        currentParts[part.partType] = part.resaurceFileName;
        SavePlayerData(); // 保存を反映
        OnAnyPartEquipped?.Invoke(); // 装備したら通知

        // 判定処理を追加
        CheckEquippedParts();
    }

    public void RemoveCurrentPart(PartType partType)
    {
        if (currentParts.ContainsKey(partType))
        {
            currentParts.Remove(partType);
            SavePlayerData(); // 保存を反映
        }
    }

    public Dictionary<PartType, string> GetAllCurrentParts()
    {
        return new Dictionary<PartType, string>(currentParts);
    }

    public string GetCurrentPartName(PartType partType)
    {
        return currentParts.TryGetValue(partType, out var partName) ? partName : null;
    }

    public bool HasAllRequiredPartsEquipped()
    {
        return currentParts.ContainsKey(PartType.Body) &&
               currentParts.ContainsKey(PartType.Rocket) &&
               currentParts.ContainsKey(PartType.Tire) &&
               currentParts.ContainsKey(PartType.Wing);
    }

    // プレイヤーデータをJSONで保存
    private void SavePlayerData()
    {
        var saveData = new PlayerSaveData
        {
            coins = playerCoins,
            purchasedPartNames = purchasedParts,
            currentParts = new List<PartTypePartPair>(),
            maxDistance = MaxDistance,
            maxAltitude = MaxAltitude
        };

        foreach (var kvp in currentParts)
        {
            saveData.currentParts.Add(new PartTypePartPair { partType = kvp.Key, partName = kvp.Value });
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
    }

    // プレイヤーデータをJSONファイルから読み込み
    public void LoadPlayerData()
    {
        if (File.Exists(SavePath)) // セーブファイルが存在するか確認
        {
            string json = File.ReadAllText(SavePath); // ファイルを読み込む
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json); // JSONをデシリアライズ

            // 読み込んだデータを反映
            playerCoins = saveData.coins;
            purchasedParts = saveData.purchasedPartNames ?? new List<string>();
            MaxDistance = saveData.maxDistance;
            MaxAltitude = saveData.maxAltitude;

            currentParts = new Dictionary<PartType, string>();
            if (saveData.currentParts != null)
            {
                foreach (var pair in saveData.currentParts)
                {
                    currentParts[pair.partType] = pair.partName; // パーツ名を保存
                }
            }
        }
        else
        {
            // セーブデータがない場合は初期化
            playerCoins = 2000;
            MaxDistance = 0f;
            MaxAltitude = 0f;
            purchasedParts = new List<string>();
            currentParts = new Dictionary<PartType, string>();

            // イベントを発火
            OnCoinsChanged?.Invoke(playerCoins);
        }
    }
    public bool IsPartPurchased(string partName)
    {
        return purchasedParts.Contains(partName);
    }

    public void ResetPlayerData()
    {
        playerCoins = 440100;
        MaxDistance = 0f;
        MaxAltitude = 0f;
        purchasedParts = new List<string>
        {
            "紙ボディ",
            "ロケット花火",
            "キャスター",
            "カミツバサ"
        };
        currentParts = new Dictionary<PartType, string>();

        if (File.Exists(SavePath))
        {
            File.Delete(SavePath); // セーブファイルを削除
            Debug.Log("セーブデータを削除しました。");
        }

        GameManager.Instance.ResetGameState(); // ゲームの状態をリセット

        // 🔽 チュートリアルフラグもリセット
        PlayerPrefs.DeleteKey("InGameTutorialCompleted");
        PlayerPrefs.Save();

        SavePlayerData(); // 初期化後に保存
        Debug.Log("プレイヤーデータを初期化しました。");

        // イベントを発火
        OnCoinsChanged?.Invoke(playerCoins);
    }

    //コインを加算するメソッド
    public void AddCoins(int amount)
    {
        if (amount < 0) return; // 負の値は無視

        playerCoins += amount;
        SavePlayerData();

        Debug.Log($"コインを {amount} 枚加算しました。現在の所持コイン: {playerCoins}");

        // イベントを発火
        OnCoinsChanged?.Invoke(playerCoins);
    }

    //最高地点を記録するメソッド
    public (bool distanceUpdated, bool altitudeUpdated) TryUpdateHighScore(float distance, float altitude)
    {
        bool distanceUpdated = false;
        bool altitudeUpdated = false;
        if (distance > MaxDistance)
        {
            MaxDistance = distance;
            distanceUpdated = true;
        }

        if (altitude > MaxAltitude)
        {
            MaxAltitude = altitude;
            altitudeUpdated = true;
        }

        if (distanceUpdated || altitudeUpdated)
        {
            SavePlayerData(); //更新されたら保存
        }

        return (distanceUpdated, altitudeUpdated);
    }

    private void CheckEquippedParts()
    {
        foreach (var kvp in currentParts)
        {
            string equippedPartName = kvp.Value;

            if (!initialParts.Contains(equippedPartName))
            {
                Debug.Log("初期パーツ以外が装備されています！");
                OnEquippedNonInitialPart?.Invoke();
                return;
            }
        }
    }
    private readonly HashSet<string> initialParts = new HashSet<string>
    {
        "冷蔵庫段ボール",
        "ロケット花火",
        "キャスター",
        "段ボール"
    };
}
