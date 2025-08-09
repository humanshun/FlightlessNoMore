using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 抽象クラス：このクラスを直接インスタンス化できず、継承して使用する
public abstract class PlayerController : MonoBehaviour
{
    // プレイヤーの状態管理
    public bool IsSit = false;                // しゃがんでいるかどうか
    public int currentJumpCount = 0;          // 現在のジャンプ回数
    public bool isGrounded = false;           // 地面に接地しているか
    public bool OnceJumpRayCheck = false;     // 一度だけジャンプ時にRayでチェックする

    public bool Is_DownJump_GroundCheck = false; // ダウンジャンプ時に下のブロックが地面かどうかの判定

    // 移動関連
    protected float m_MoveX;
    public Rigidbody2D m_rigidbody;
    protected CapsuleCollider2D m_CapsulleCollider;
    protected Animator m_Anim;

    [Header("[Setting]")]
    public float MoveSpeed = 6;      // 移動速度
    public int JumpCount = 2;        // 最大ジャンプ回数（例：2なら2段ジャンプ可能）
    public float jumpForce = 15f;    // ジャンプの力

    /// <summary>
    /// アニメーションの状態を更新
    /// </summary>
    protected void AnimUpdate()
    {
        // 現在のアニメーションが "Attack" でない場合にアニメーションを切り替える
        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (Input.GetKey(KeyCode.Mouse0)) // マウスの左クリックで攻撃
            {
                m_Anim.Play("Attack");
            }
            else
            {
                if (m_MoveX == 0) // 移動していない場合
                {
                    if (!OnceJumpRayCheck)
                        m_Anim.Play("Idle"); // アイドル状態
                }
                else
                {
                    m_Anim.Play("Run"); // 移動中
                }
            }
        }
    }

    /// <summary>
    /// キャラクターの向きを変更する（左向きなら `bLeft` を `true`）
    /// </summary>
    protected void Filp(bool bLeft)
    {
        transform.localScale = new Vector3(bLeft ? 1 : -1, 1, 1);
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    protected void prefromJump()
    {
        m_Anim.Play("Jump");

        // 一旦速度をリセット
        m_rigidbody.linearVelocity = Vector2.zero;

        // 上向きに力を加えてジャンプさせる
        m_rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // ジャンプ中フラグ
        OnceJumpRayCheck = true;
        isGrounded = false;

        // ジャンプ回数をカウント
        currentJumpCount++;
    }

    // 床との接地をレイキャストでチェックするための変数
    Vector2 RayDir = Vector2.down;
    float PretmpY;
    float GroundCheckUpdateTic = 0;
    float GroundCheckUpdateTime = 0.01f;

    /// <summary>
    /// 地面に接触しているかどうかのチェックを更新
    /// </summary>
    protected void GroundCheckUpdate()
    {
        if (!OnceJumpRayCheck) return; // ジャンプ時のみ判定

        GroundCheckUpdateTic += Time.deltaTime;

        if (GroundCheckUpdateTic > GroundCheckUpdateTime)
        {
            GroundCheckUpdateTic = 0;

            if (PretmpY == 0)
            {
                PretmpY = transform.position.y;
                return;
            }

            float reY = transform.position.y - PretmpY;  // 高さの変化量を計算

            if (reY <= 0) // 落下中の場合
            {
                if (isGrounded)
                {
                    LandingEvent(); // 地面に着地した場合の処理（派生クラスで実装）
                    OnceJumpRayCheck = false;
                }
                else
                {
                    Debug.Log("まだ地面に接触していない");
                }
            }

            PretmpY = transform.position.y;
        }
    }

    /// <summary>
    /// 着地時のイベント（派生クラスで実装する必要あり）
    /// </summary>
    protected abstract void LandingEvent();
}
