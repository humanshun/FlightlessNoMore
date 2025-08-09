using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Cysharp.Threading.Tasks;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;
    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // これを追加
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async UniTask ChangeScene(string sceneName, float fadeOutTime = 1.0f, float fadeInTime = 1.0f)
    {
        // フェードアウト
        await Fade(0f, 1f, fadeOutTime);
        await UniTask.DelayFrame(1); // シーン切り替え前に1フレーム待つ
        SceneManager.LoadScene(sceneName);
        await UniTask.DelayFrame(1); // シーンロード後に1フレーム待つ（Canvas再取得のため）
        await Fade(1f, 0f, fadeInTime);
    }

    private async UniTask Fade(float from, float to, float time)
    {
        float t = 0f;
        Color c = fadeImage.color;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, t / time);
            fadeImage.color = new Color(c.r, c.g, c.b, a);
            await UniTask.Yield();
        }
        fadeImage.color = new Color(c.r, c.g, c.b, to);
    }
}
