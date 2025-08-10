using UnityEngine;

public class StartButton : MonoBehaviour
{
    private bool isProcessing;
    private async void OnMouseDown()
    {
        if (isProcessing) return; // 二重押しガード
        isProcessing = true;

        PlayerData.Instance.ResetPlayerData();
        PlayerData.Instance.IsStarted = true;
        await SceneChanger.Instance.ChangeScene("Custom", 1.0f, 1.0f);
    }
}
