// Copyright (C) 2015 ricimi - All rights reserved.
// このコードは Unity Asset Store のエンドユーザーライセンス契約に基づいてのみ使用できます。
// 詳細は http://unity3d.com/company/legal/as_terms を参照してください。

using UnityEngine;
using UnityEngine.UI;

namespace Ricimi
{
    // SceneTransition クラスは、シーン遷移（フェードアウト/フェードインなどの演出付き）を担当します。
    // 実際のシーンの切り替え処理は Transition クラスで行われます。
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private Button playButton; // シーン遷移をトリガーするボタン
        private void Start()
        {
            playButton.onClick.AddListener(PerformTransition);
        }
        // 遷移先のシーン名。インスペクタから設定可能です。
        public string scene = "<Insert scene name>";

        // シーン遷移にかかる時間（秒単位）。例: 1.0f は1秒間の遷移を意味します。
        public float duration = 1.0f;

        // シーン遷移時に使用するフェードカラー。デフォルトは黒色。
        public Color color = Color.black;

        // PerformTransition メソッドは、シーン遷移を実行するためのメソッドです。
        public void PerformTransition()
        {
            // Transition クラスの LoadLevel メソッドを呼び出して、
            // 指定したシーン名、遷移時間、フェードカラーを用いてシーン切り替えを実行します。
            AudioManager.Instance.PlaySFX("SE_ButtonLow");
            Transition.LoadLevel(scene, duration, color);
        }
    }
}
