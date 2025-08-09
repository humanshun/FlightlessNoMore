using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Popup : MonoBehaviour
{
    // 背景の色を設定（RGB: 非常に暗い色、アルファ: 0.6 → 半透明）
        public Color backgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);

        // 生成される背景の GameObject を保持するための変数
        private GameObject m_background;

        // ポップアップを開く際に呼び出すメソッド
        public void Open()
        {
            // 背景を追加してフェードインさせる処理を実行
            AddBackground();
        }

        // ポップアップを閉じる際に呼び出すメソッド
        public void Close()
        {
            // この GameObject にアタッチされている Animator コンポーネントを取得
            var animator = GetComponent<Animator>();

            // 現在のアニメーション状態が "Open" であれば "Close" アニメーションを再生
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
                animator.Play("Close");

            // 背景のフェードアウトを実行
            RemoveBackground();

            // 一定時間後にポップアップと背景を破棄するためのコルーチンを開始
            StartCoroutine(RunPopupDestroy());
        }

        // ポップアップの破棄を行うコルーチン（0.5秒後に破棄）
        private IEnumerator RunPopupDestroy()
        {
            // 0.5秒待機
            yield return new WaitForSeconds(0.5f);
            // 背景の GameObject を破棄
            Destroy(m_background);
            // ポップアップ自体の GameObject を破棄
            Destroy(gameObject);
        }

        // 背景を追加し、フェードインさせる処理
        private void AddBackground()
        {
            // 1x1ピクセルのテクスチャを作成
            var bgTex = new Texture2D(1, 1);
            // テクスチャの唯一のピクセルに背景色を設定
            bgTex.SetPixel(0, 0, backgroundColor);
            // テクスチャに変更を適用
            bgTex.Apply();

            // "PopupBackground" という名前で背景用の GameObject を生成
            m_background = new GameObject("PopupBackground");
            // 背景に Image コンポーネントを追加（UI 表示用）
            var image = m_background.AddComponent<Image>();

            // 1x1サイズの矩形情報を作成
            var rect = new Rect(0, 0, bgTex.width, bgTex.height);
            // テクスチャから Sprite を生成（Pivot は中央）
            var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);

            // Image のマテリアルにテクスチャを設定
            image.material.mainTexture = bgTex;
            // Image に生成した Sprite を設定
            image.sprite = sprite;

            // Image の初期色（設定済みの色）をそのまま使用（※この処理は場合により不要かもしれません）
            var newColor = image.color;
            image.color = newColor;

            // CanvasRenderer のアルファを初期状態で 0 に設定（完全に透明）
            image.canvasRenderer.SetAlpha(0.0f);
            // 0.4秒かけてアルファ値を 1 にフェードイン（不透明にする）
            image.CrossFadeAlpha(1.0f, 0.4f, false);

            // シーン内の "Canvas" という名前の GameObject を取得
            var canvas = GameObject.Find("Canvas");

            // 背景のスケールを標準 (1,1,1) に設定
            m_background.transform.localScale = new Vector3(1, 1, 1);
            // 背景の RectTransform サイズを Canvas のサイズに合わせる
            m_background.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
            // 背景の親を Canvas に設定し、UI ツリーに組み込む
            m_background.transform.SetParent(canvas.transform, false);
            // 背景の表示順序を、ポップアップと同じ階層位置に設定
            m_background.transform.SetSiblingIndex(transform.GetSiblingIndex());
        }

        // 背景をフェードアウトさせる処理
        private void RemoveBackground()
        {
            // 背景の Image コンポーネントを取得
            var image = m_background.GetComponent<Image>();
            // Image コンポーネントが存在する場合、0.2秒でフェードアウト（アルファを 0 に）
            if (image != null)
                image.CrossFadeAlpha(0.0f, 0.2f, false);
        }
}
