// Copyright (C) 2015 ricimi - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.UI;

namespace Ricimi
{
    // このクラスは、指定されたプレハブのポップアップを作成し、
    // 現在のシーンのUIキャンバスに追加して開く役割を担います。
    public class PopupOpener : MonoBehaviour
    {
        // ポップアップのプレハブを格納するためのパブリック変数
        public GameObject popupPrefab;
        [SerializeField] private Button button;

        // キャンバスを格納するための保護された変数
        protected Canvas m_canvas;

        // スクリプトが有効になったときに最初に呼び出されるメソッド
        protected void Start()
        {
            // 親オブジェクトからCanvasコンポーネントを取得し、m_canvasに格納
            m_canvas = GetComponentInParent<Canvas>();

            button.onClick.AddListener(OpenPopup);
        }

        // ポップアップを生成して表示するためのメソッド
        public virtual void OpenPopup()
        {
            // popupPrefabをインスタンス化し、新しいゲームオブジェクトとしてpopupに格納
            var popup = Instantiate(popupPrefab) as GameObject;
            // ポップアップをアクティブにする
            popup.SetActive(true);
            // ポップアップのスケールをゼロに設定（後でアニメーションで拡大するため）
            popup.transform.localScale = Vector3.zero;
            // ポップアップをキャンバスの子オブジェクトとして設定
            popup.transform.SetParent(m_canvas.transform, false);
            // ポップアップのPopupコンポーネントのOpenメソッドを呼び出して、ポップアップを開く
            popup.GetComponent<Popup>().Open();
        }
    }
}