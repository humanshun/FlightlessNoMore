using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System;
using System.Collections;
using Unity.Mathematics;
using Cysharp.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

public class PlayerController2 : MonoBehaviour
/*
    各パーツのステータス
    bodyData
        重量（ぶつかった時の影響）
        空気抵抗（スピード減衰率）
        体力（HP）
    rocketData
        重量（ぶつかった時の影響）
        噴射力（スピード上昇率）
        噴射時間（噴射できる時間）
    tireData
        重量（ぶつかった時の影響）
        空気抵抗（スピード減衰率）
        地上加速（地上でのスピード上昇率）
    wingData
        重量（ぶつかった時の影響）
        空気抵抗（スピード減衰率）
        空中コントロール（空中での回転力）
*/
{
    // ===== パーツのプレハブ =====
    public GameObject rocketThrustPrefab; // エフェクトのプレハブ
    public GameObject dieEffectPrefab; // 死亡エフェクトのプレハブ
    public Vector2 effectPosition;
    private GameObject currentRocketThrustInstance;
    private GameObject currentDieEffectInstance;

    // ===== 各パーツデータ参照 =====
    private BodyData bodyData; // 本体のデータ
    private RocketData rocketData; // ロケットのデータ
    private TireData tireData; // タイヤのデータ
    private WingData wingData; // 翼のデータ

    // ===== パーツごとの合計値 =====
    private float total_Weight; // 総重量
    private float total_AirResistance; //総空気抵抗
    private float total_Health; // 総体力
    private float total_JetThrust; // 総空中加速度
    private float total_Torque; // 総地上加速度
    private float total_AirControl; //空中コントロール
    private float total_RocketTime; // ロケットの合計噴射時間

    // ===== プレイヤーゲッター =====
    public float TotalHealth => total_Health; // 総体力のゲッター
    public float TotalRocketTime => total_RocketTime; // ロケットの合計噴射時間のゲッター
    public float TotalWeight => total_Weight;

    // ===== イベント =====
    public event Action<float> OnHealthChanged;
    public event Action<float> OnRocketTimeChanged;

    // ===== プレイヤー制御用パラメータ =====
    [SerializeField] private Rigidbody2D rb; // プレイヤーのRigidbody2D
    [SerializeField] private float playerAngle;
    private bool isRocketKeyDown = false;
    private float crrentRocketTime; //現在のロケット噴射時間
    private bool finishRocketTime = true; //ロケットが使えるかどうか
    [SerializeField] private LayerMask groundLayer; //GroundCheckのBoxCollider2D
    public BoxCollider2D groundCheckCollider; //GroundCheckのBoxCollider2D
    public bool groundCheck; //Groundについているかどうか
    private Vector2 rightDirection;// プレイヤーの回転に基づいた右方向

    // ===== 水判定・水中用 =====
    [SerializeField] private LayerMask waterLayer; // Waterレイヤーを指定
    [SerializeField] private BoxCollider2D playerCollider; // プレイヤーのCapsuleCollider2D
    private bool inWater; // 水中にいるかどうか
    float MovY = 0;

    // ===== 水中物理パラメータ =====]
    [SerializeField] private float waterSurfaceY = -40f; // 水面の高さ
    [SerializeField] private float waterLinearDamping = 1.0f;  // 水中での抵抗値

    // ===== 無敵・点滅・エフェクト =====
    private float invincibleTime = 1.0f; // 無敵時間
    private float blinkInterval = 0.1f;   // 点滅間隔
    private bool isInvincible = false;    // 無敵状態かどうか
    private bool rocketPermanentlyDisabled = false; // ロケット永久封印
    private SpriteRenderer[] spriteRenderers; // スプライトレンダラー

    // ===== 死亡・クリア判定 =====
    private bool isDead = false; // 死亡状態かどうか

    [SerializeField] private float clearX = 7000f; // ゴール地点X
    [SerializeField] private float clearSpeedThreshold = 1f; // 停止とみなす速度
    [SerializeField] private LayerMask runwayLayer; // 滑走路のレイヤー
    private bool isClear = false; // クリア状態かどうか

    // ===== サウンド関連 =====
    [SerializeField] private string rocketLoopSFXName = "SE_Rocket"; // SoundDataに登録した名前
    [SerializeField] private string carLoopSFXName = "SE_Car";      // SoundDataに登録した名前
    [SerializeField] private string flyLoopSFXName = "SE_Fly";      // SoundDataに登録した名前
    private bool isCarSFXPlaying = false;
    private bool isFlySFXPlaying = false;
    [SerializeField] private float maxCarSpeed = 50f; // 最大音量になる速度
    [SerializeField] private float maxFlySpeed = 200f;
    [SerializeField] private float minVolume = 0.1f;
    [SerializeField] private float maxVolume = 1.0f;

    // ===== 姿勢安定化用パラメータ =====
    [SerializeField] private float timeToZero = 0.2f;  // 角速度を0にするまでの時間（秒）
    private const float targetAngularVelocity = 0f;
    void Awake()
    {
        Application.targetFrameRate = 60; // フレームレートを60に固定

        //プレイヤーのパーツデータを取得
        GetPartsData();

        // 初期ステータスを計算
        UpdateStats();
    }

    void Start()
    {
        // Rigidbody2D を取得
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(WaitAndInitSprites());
    }
    private IEnumerator WaitAndInitSprites()
    {
        yield return null; // 1フレーム待つ

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }
    void Update()
    {
        // ゲームオーバー中なら一切の入力処理を無視
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
        if (GameManager.Instance != null && GameManager.Instance.isTutorial) return;

        MovY = Input.GetAxis("Vertical");

        // プレイヤーのローカルZ回転を取得
        playerAngle = transform.localRotation.eulerAngles.z;

        //ブーストメソッド
        OnSpaceClick();

        //地面に接触しているかどうかを確認
        GroundCheck();

        //地上で加速するメソッド
        GroundAddForce();
        UpdateCarLoopSFX();

        // プレイヤーの回転に基づいた右方向を計算
        rightDirection = new Vector2(Mathf.Cos(playerAngle * Mathf.Deg2Rad), Mathf.Sin(playerAngle * Mathf.Deg2Rad));

        //水の中にいるか判定
        InWater();

        CheckGameOver();

        CheckClear();
    }

    void FixedUpdate()
    {
        // ゲームオーバー中なら一切の入力処理を無視
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        // プレイヤーの向きを制御する
        PlayerAngle();

        RotateToMoveDirectionWithControl();
    }

    // ===== プレイヤーのパーツ性能を集計 ===== 
    public void UpdateStats()
    {
        total_Weight = 0f;
        total_Torque = 0f;
        total_JetThrust = 0f;
        total_RocketTime = 0f;
        total_AirResistance = 0f;
        total_AirControl = 0f;
        total_Health = 0f;


        if (bodyData != null)
        {
            total_Weight += bodyData.weight.value;
            total_AirResistance += bodyData.airResistance.value;
            total_Health += bodyData.hp.value;
        }
        if (wingData != null)
        {
            total_Weight += wingData.weight.value;
            total_AirResistance += wingData.airResistance.value;
            total_AirControl += wingData.airControl.value;
        }
        if (tireData != null)
        {
            total_Weight += tireData.weight.value;
            total_AirResistance += tireData.airResistance.value;
            total_Torque += tireData.torque.value;
        }
        if (rocketData != null)
        {
            total_Weight += rocketData.weight.value;
            total_JetThrust += rocketData.jetThrust.value;
            total_RocketTime += rocketData.jetTime.value;
        }

        if (bodyData != null && rocketData != null)
        {
            effectPosition = bodyData.rocketPosition + rocketData.effectPosition;
        }

        Debug.Log(
            $"総重量     :{total_Weight},\n " +
            $"総空気抵抗  :{total_AirResistance}, \n " +
            $"体力       :{total_Health}, \n " +
            $"コントロール:{total_AirControl}, \n " +
            $"地上加速    :{total_Torque}, \n " +
            $"ブースト    :{total_JetThrust}, \n " +
            $"噴射時間    :{total_RocketTime}"
            );

        rb.linearDamping = total_AirResistance; //空気抵抗。下を向くほどその方向に加速するように。
        // rb.mass = total_Weight;
    }

    // ===== プレイヤーの方向や力を加えるメソッド =====
    /*
        プレイヤーの空中姿勢制御・揚力付与・回転減衰を行うメソッド。
        ・地上にいる間や死亡時は何もしない。
        ・上下入力(MovY)とパーツ性能(total_AirControl)に応じて回転トルクを加える。
        ・進行方向が下向きのときに揚力（上向きの力）を加え、飛行感を演出。
        ・角速度（回転の速さ）を徐々に0に近づけて、空中での姿勢を安定させる。
        → これにより、プレイヤーは空中で直感的かつ自然な操作感を得られる。
    */
    public void PlayerAngle()
    {
        if (isDead) return;
        if (groundCheck) return;

        // 上下入力とパーツ性能に応じて回転トルクを加える
        float torque = MovY * total_AirControl;  // MovYとtotal_AirControlで回転力を計算
        rb.AddTorque(torque);

        // 進行方向が下向きのときに揚力（上向きの力）を加える
        float thrustForce = Vector2.Dot(rb.linearVelocity, rb.GetRelativeVector(Vector2.down));
        Vector2 relForce = Vector2.up * thrustForce; // 上方向ベクトルに変換
        rb.AddForce(rb.GetRelativeVector(relForce)); // Rigidbody2Dに揚力を加える

        // 現在の角速度を取得
        float currentAngularVelocity = rb.angularVelocity;

        // 角速度が一定以上なら徐々に0に近づける（姿勢安定化）
        if (Mathf.Abs(currentAngularVelocity) > 0.01f)
        {
            rb.angularVelocity = Mathf.MoveTowards(
                currentAngularVelocity,
                targetAngularVelocity,
                Mathf.Abs(currentAngularVelocity) / timeToZero * Time.deltaTime
            );
        }
        else
        {
            // 角速度が十分小さくなったら完全に0に設定
            rb.angularVelocity = 0;
        }
    }

    // ===== ロケットメソッド =====
    public void OnSpaceClick()
    {
        if (isDead) return; // ゲームオーバー中なら何もしない
        if (rocketPermanentlyDisabled) return; // ← 追加
        if (inWater) return; // 水中ではロケットを使用できない
        if (!finishRocketTime) return; // ロケットの噴射時間が終了しているか、ロケットを使用できない

        bool isKeyHeld = Input.GetKey(KeyCode.Space);

        // 左シフトを押したときに
        if (isKeyHeld)
        {
            if (!isRocketKeyDown)
            {
                StartRocketEffect();
                isRocketKeyDown = true;
            }
            // ブーストしてる秒数をカウントして
            crrentRocketTime += Time.deltaTime;

            // もしブースト秒数が余ってれば
            if (total_RocketTime > crrentRocketTime)
            {
                // ロケット噴射時間を更新
                OnRocketTimeChanged?.Invoke(total_RocketTime - crrentRocketTime);
                // 回転に基づいて力の方向を計算
                Vector2 forceDirection = new Vector2(Mathf.Cos(playerAngle * Mathf.Deg2Rad), Mathf.Sin(playerAngle * Mathf.Deg2Rad));
                // プレイヤーの正面方向に力を加える
                rb.AddForce(forceDirection * total_JetThrust, ForceMode2D.Force);
            }
            else
            {
                //ロケット噴射時間が終了
                finishRocketTime = false;
                //エフェクト停止
                StopRocketEffect();
                isRocketKeyDown = false;
            }
        }
        else
        {
            if (isRocketKeyDown)
            {
                StopRocketEffect();
                isRocketKeyDown = false;
            }
        }
    }

    // ===== ロケットのエフェクトを開始するメソッド =====
    private void StartRocketEffect()
    {
        currentRocketThrustInstance = Instantiate(rocketThrustPrefab, transform);
        currentRocketThrustInstance.transform.localPosition = effectPosition; // 背面に配置
        AudioManager.Instance.PlayRocketLoopSFX(rocketLoopSFXName);
    }
    // ===== ロケットのエフェクトを停止するメソッド =====
    private void StopRocketEffect()
    {
        if (currentRocketThrustInstance == null) return;

        ParticleSystem ps = currentRocketThrustInstance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        AudioManager.Instance.StopRocketLoopSFX();
    }


    // ===== 地面に接触しているかどうかを確認するメソッド =====
    private void GroundCheck()
    {
        if (groundCheckCollider == null) return;
        // 地面に接触しているかどうかを確認
        groundCheck = groundCheckCollider.IsTouchingLayers(groundLayer);
    }

    // ===== 車輪加速メソッド =====
    private void GroundAddForce()
    {
        // ゲームオーバー中なら一切の入力処理を無視
        if (isDead) return;
        if (!groundCheck) return;
        // 水平方向の入力取得（Aキー: -1, Dキー: 1, それ以外: 0）
        float input = Input.GetAxis("Horizontal");

        if (input != 0)
        {
            // 入力に基づいて力を計算
            Vector2 force = rightDirection * input * total_Torque;

            // 力を加える
            rb.AddForce(force, ForceMode2D.Force);
        }
    }

    // ===== 車のループサウンドを更新するメソッド =====
    private void UpdateCarLoopSFX()
    {
        float moveThreshold = 0.5f; // 走行とみなす最低速度
        float currentSpeed = rb.linearVelocity.magnitude;
        bool shouldPlayCar = groundCheck && currentSpeed > moveThreshold;
        bool shouldPlayFly = currentSpeed > moveThreshold;

        // 車サウンド（地面にいて速度がしきい値超え）
        if (shouldPlayCar)
        {
            if (!isCarSFXPlaying)
            {
                AudioManager.Instance.PlayCarLoopSFX(carLoopSFXName);
                isCarSFXPlaying = true;
            }
            float t = Mathf.InverseLerp(0f, maxCarSpeed, currentSpeed); // 0～1に正規化
            float carVolume = Mathf.Lerp(minVolume, maxVolume, t);
            AudioManager.Instance.SetCarLoopVolume(carVolume);
        }
        else
        {
            if (isCarSFXPlaying)
            {
                AudioManager.Instance.StopCarLoopSFX();
                isCarSFXPlaying = false;
            }
        }

        // 飛行サウンド（地面関係なく速度がしきい値超え）
        if (shouldPlayFly)
        {
            if (!isFlySFXPlaying)
            {
                AudioManager.Instance.PlayFlyLoopSFX(flyLoopSFXName);
                isFlySFXPlaying = true;
            }
            float g = Mathf.InverseLerp(0f, maxFlySpeed, currentSpeed);
            float flyVolume = Mathf.Lerp(minVolume, maxVolume, g);
            AudioManager.Instance.SetFlyLoopVolume(flyVolume);
        }
        else
        {
            if (isFlySFXPlaying)
            {
                AudioManager.Instance.StopFlyLoopSFX();
                isFlySFXPlaying = false;
            }
        }
    }


    private void GetPartsData()
    {
        var currentPart = PlayerData.Instance.GetAllCurrentParts();

        if (currentPart.TryGetValue(PartType.Body, out var bodyResourceName))
        {
            bodyData = Resources.Load<BodyData>($"Parts/{bodyResourceName}");
        }
        if (currentPart.TryGetValue(PartType.Rocket, out var rocketResourceName))
        {
            rocketData = Resources.Load<RocketData>($"Parts/{rocketResourceName}");
        }
        if (currentPart.TryGetValue(PartType.Tire, out var tireResourceName))
        {
            tireData = Resources.Load<TireData>($"Parts/{tireResourceName}");
        }
        if (currentPart.TryGetValue(PartType.Wing, out var wingResourceName))
        {
            wingData = Resources.Load<WingData>($"Parts/{wingResourceName}");
        }
    }

    // ===== 水中にいるかどうかを判定するメソッド =====
    private void InWater()
    {
        // 水中にいるか判定
        bool wasInWater = inWater;
        inWater = playerCollider.IsTouchingLayers(waterLayer);

        if (inWater && !wasInWater)
        {
            // 水に入った瞬間だけ呼ぶ
            AudioManager.Instance.PlayWaterLoopSFX("SE_InWater");

            if (rb.linearVelocity.magnitude > 50f)
            {
                AudioManager.Instance.PlaySFX("SE_Water");
            }
            StopRocketEffect();
            rocketPermanentlyDisabled = true; // ← 永久封印

            //水中抵抗を再現
            rb.linearDamping = waterLinearDamping;
        }
        else if (!inWater && wasInWater)
        {
            // 水から出た瞬間だけ呼ぶ
            AudioManager.Instance.StopWaterLoopSFX();
            //通常抵抗値に戻す
            rb.linearDamping = total_AirResistance;
        }

        // 毎フレーム浮力は加える（inWater中のみ）
        if (inWater)
        {
            ApplyRealisticBuoyancy();
        }
    }

    // 水中で浮力を加えるメソッド
    private void ApplyRealisticBuoyancy()
    {
        // プレイヤーの上下の位置を計算
        float bottomY = transform.position.y - playerCollider.bounds.extents.y;
        float topY = transform.position.y + playerCollider.bounds.extents.y;
        float height = topY - bottomY;

        float submergedRatio = 0f;

        // プレイヤーが完全に水面の上
        if (bottomY > waterSurfaceY)
        {
            submergedRatio = 0f;
        }
        // プレイヤーが完全に水中
        else if (topY < waterSurfaceY)
        {
            submergedRatio = 1f;
        }
        // 半分だけ浸かってる場合
        else
        {
            float submergedHeight = waterSurfaceY - bottomY;
            submergedRatio = Mathf.Clamp01(submergedHeight / height);
        }

        if (submergedRatio > 0f)
        {
            // プレイヤーの重さに比例した浮力（重さと釣り合う）
            float gravityForce = rb.mass * Mathf.Abs(Physics2D.gravity.y);
            float buoyancyForce = gravityForce * submergedRatio;

            // 上方向に浮力を加える
            rb.AddForce(Vector2.up * buoyancyForce, ForceMode2D.Force);

            // 水の粘性抵抗 → 水中の動きを鈍くする
            rb.linearVelocity *= Mathf.Lerp(1f, 0.9f, submergedRatio);
        }
    }
    // ===== ゲームオーバーのチェックメソッド =====

    private void CheckGameOver()
    {
        if (transform.position.x >= 260f && rb.linearVelocity.magnitude < 1f && inWater)
        {
            GameManager.Instance.GameOver();
            rb.linearVelocity = Vector2.zero; // プレイヤーの速度をゼロにする
            rb.bodyType = RigidbodyType2D.Kinematic; // Rigidbodyをキネマティックにして動かなくする
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // 全ての動きを制限する
        }
    }

    // ===== クリア判定メソッド =====
    private void CheckClear()
    {
        if (isClear) return;

        bool reachedGoal = transform.position.x >= clearX;
        bool isStopped = rb.linearVelocity.magnitude < clearSpeedThreshold;
        if (reachedGoal && isStopped && groundCheck)
        {
            isClear = true;

            rb.linearVelocity = Vector2.zero; // プレイヤーの速度をゼロにする
            rb.bodyType = RigidbodyType2D.Kinematic; // Rigidbodyをキネマティックにして動かなくする
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // 全ての動きを制限する

            GameManager.Instance.GameClear();
        }
    }

    // ===== プレイヤーの体力を取得するメソッド =====
    public void TakeDamage(float damage)
    {
        // 無敵状態ならダメージを無視
        if (isInvincible) return;

        // ダメージを受けたときの処理
        Debug.Log("プレイヤーがダメージを受けました: " + damage);
        if (total_Health > 0)
        {
            total_Health -= damage;
            total_Health = Mathf.Max(total_Health, 0); // 体力が0未満にならないようにする

            // 体力が変化したことを通知
            OnHealthChanged?.Invoke(total_Health);

            // 体力が0以下になったらプレイヤーを死亡させる
            if (total_Health <= 0)
            {
                Die();
            }
        }

        _ = InvincibilityCoroutine();
    }

    // ===== プレイヤーが死亡したときの処理 =====
    private void Die()
    {
        //プレイヤーを操作できなくする
        isDead = true;
        StopRocketEffect();
        // ループサウンドをすべて停止
        AudioManager.Instance.StopCarLoopSFX();
        AudioManager.Instance.StopFlyLoopSFX();
        AudioManager.Instance.StopWaterLoopSFX();
        currentDieEffectInstance = Instantiate(dieEffectPrefab, transform);
        currentDieEffectInstance.transform.localPosition = effectPosition; // 背面に配置
    }

    // ===== 無敵コルーチン =====
    private async UniTask InvincibilityCoroutine()
    {
        isInvincible = true;

        float timer = 0f;

        while (timer < invincibleTime)
        {
            SetRenderersVisible(false);
            await UniTask.Delay((int)(blinkInterval * 1000), cancellationToken: this.GetCancellationTokenOnDestroy());
            SetRenderersVisible(true);
            await UniTask.Delay((int)(blinkInterval * 1000), cancellationToken: this.GetCancellationTokenOnDestroy());

            timer += blinkInterval * 2;
        }

        isInvincible = false;
    }

    // ===== スプライトの表示切替メソッド =====
    private void SetRenderersVisible(bool visible)
    {
        foreach (var sr in spriteRenderers)
        {
            sr.enabled = visible;
        }
    }

    // ===== 進行方向に回転補正するメソッド =====
    /*
        プレイヤーの進行方向（速度ベクトル）と現在の回転角度の差を計算し、
        その差を補正するトルクを加えることで、機体が進行方向を向くように自動で姿勢制御します。
        ・速度がほぼゼロの場合は何もしません。
        ・水中では補正トルクを極端に弱くし、地上や空中では強く補正します。
        ・速度が速いほど補正トルクも強くなります。
        これにより、飛行時や走行時に機体が自然に進行方向を向く挙動を実現します。
    */
    private void RotateToMoveDirectionWithControl()
    {
        Vector2 velocity = rb.linearVelocity;

        if (velocity.sqrMagnitude < 0.01f)
            return;

        float moveAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg; // 進行方向の角度
        float currentAngle = rb.rotation; // 現在の機体の角度

        float angleDifference = Mathf.DeltaAngle(currentAngle, moveAngle); // 角度差
        float speedFactor = velocity.magnitude * 0.005f; // 速度に応じた補正強度
        float waterMultiplier = inWater ? 0.01f : 2.0f; // 水中なら補正を弱く

        float correctionTorque = angleDifference * speedFactor * waterMultiplier; // 最終的な補正トルク

        rb.AddTorque(correctionTorque); // トルクを加えて姿勢を補正
    }

    // ===== プレイヤーの体力を回復するメソッド =====
    public void Heal(float healAmount)
    {
        if (isDead) return; // 死亡時は回復しない

        float beforeHealth = total_Health;

        // 回復量を加算
        total_Health += healAmount;

        // 最大体力はパーツ構成で決まるなら「最大値を保持」しておくべき
        // 例えば bodyData.hp.value がベースならこんな感じ
        float maxHealth = 0f;
        if (bodyData != null) maxHealth += bodyData.hp.value;
        // 他のパーツが体力に影響するなら加算

        // 最大体力を超えないようにクランプ
        total_Health = Mathf.Min(total_Health, maxHealth);

        Debug.Log($"回復: {healAmount}, 体力 {beforeHealth} → {total_Health}");

        // UI に更新を通知
        OnHealthChanged?.Invoke(total_Health);
    }
    
    // ===== ロケット噴射時間を回復するメソッド =====
    public void RecoverRocketTime(float recoverAmount)
    {
        if (rocketPermanentlyDisabled) return;

        float before = total_RocketTime - crrentRocketTime; // 残り時間

        // 消費した時間を減らす（＝残り時間を増やす）
        crrentRocketTime -= recoverAmount;
        if (crrentRocketTime < 0f) crrentRocketTime = 0f;

        float after = total_RocketTime - crrentRocketTime;

        Debug.Log($"ロケット回復: {recoverAmount}, 残り噴射時間 {before} → {after}");

        OnRocketTimeChanged?.Invoke(after);

        // 使えない状態なら再度使えるようにする
        finishRocketTime = true;
    }
}
