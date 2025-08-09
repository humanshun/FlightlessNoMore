using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordman : PlayerController
{
    private void Start()
    {
        // 必要なコンポーネントを取得
        m_CapsulleCollider = this.transform.GetComponent<CapsuleCollider2D>();
        m_Anim = this.transform.Find("model").GetComponent<Animator>();
        m_rigidbody = this.transform.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 入力のチェック
        checkInput();

        // 速度が30を超えた場合、速度を減少させる
        if (m_rigidbody.linearVelocity.magnitude > 30)
        {
            m_rigidbody.linearVelocity = new Vector2(m_rigidbody.linearVelocity.x - 0.1f, m_rigidbody.linearVelocity.y - 0.1f);
        }
    }

    public void checkInput()
    {
        // 下キーが押されたとき
        if (Input.GetKeyDown(KeyCode.S))
        {
            IsSit = true;
            m_Anim.Play("Sit");
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            m_Anim.Play("Idle");
            IsSit = false;
        }

        // 水平方向の入力を取得
        m_MoveX = Input.GetAxis("Horizontal");

        // 地面チェックの更新
        GroundCheckUpdate();

        // 攻撃アニメーション中でない場合
        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            // 左クリックが押されたとき
            if (Input.GetKey(KeyCode.Mouse0))
            {
                m_Anim.Play("Attack");
            }
            else
            {
                // 移動入力がない場合
                if (m_MoveX == 0)
                {
                    if (!OnceJumpRayCheck)
                        m_Anim.Play("Idle");
                }
                else
                {
                    m_Anim.Play("Run");
                }
            }
        }

        // 1キーが押されたとき
        if (Input.GetKey(KeyCode.Alpha1))
        {
            m_Anim.Play("Die");
        }

        // Dキーが押されたとき
        if (Input.GetKey(KeyCode.D))
        {
            // 地面にいる場合
            if (isGrounded)
            {
                if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    return;

                transform.Translate(Vector2.right * m_MoveX * MoveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(new Vector3(m_MoveX * MoveSpeed * Time.deltaTime, 0, 0));
            }

            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                return;

            if (!Input.GetKey(KeyCode.A))
                Filp(false);
        }
        // Aキーが押されたとき
        else if (Input.GetKey(KeyCode.A))
        {
            // 地面にいる場合
            if (isGrounded)
            {
                if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    return;

                transform.Translate(Vector2.right * m_MoveX * MoveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(new Vector3(m_MoveX * MoveSpeed * Time.deltaTime, 0, 0));
            }

            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                return;

            if (!Input.GetKey(KeyCode.D))
                Filp(true);
        }

        // スペースキーが押されたとき
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                return;

            if (currentJumpCount < JumpCount)
            {
                if (!IsSit)
                {
                    prefromJump();
                }
            }
        }
    }

    // 着地イベント
    protected override void LandingEvent()
    {
        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Run") && !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            m_Anim.Play("Idle");
    }
}
