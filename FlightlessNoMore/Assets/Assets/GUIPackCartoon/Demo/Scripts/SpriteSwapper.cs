// Copyright (C) 2015 ricimi - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.UI;

namespace Ricimi
{
    // UIのImageコンポーネントのスプライトを2つの事前定義された値の間で切り替えるユーティリティクラス
    public class SpriteSwapper : MonoBehaviour
    {
        // 有効状態のスプライト
        public Sprite enabledSprite;
        // 無効状態のスプライト
        public Sprite disabledSprite;

        // スプライトが切り替わっているかどうかを示すフラグ
        private bool m_swapped = true;

        // Imageコンポーネントへの参照
        private Image m_image;

        // Awakeメソッドはオブジェクトが有効化されたときに呼び出される
        public void Awake()
        {
            // Imageコンポーネントを取得
            m_image = GetComponent<Image>();
        }

        // スプライトを切り替えるメソッド
        public void SwapSprite()
        {
            // 現在の状態に応じてスプライトを切り替える
            if (m_swapped)
            {
                // 無効状態に切り替える
                m_swapped = false;
                m_image.sprite = disabledSprite;
            }
            else
            {
                // 有効状態に切り替える
                m_swapped = true;
                m_image.sprite = enabledSprite;
            }
        }
    }
}