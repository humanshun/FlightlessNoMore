using UnityEngine;

public class ContenuButton : MonoBehaviour
{
    [SerializeField] private GameObject blackPanel;

    private void Start()
    {
        if (PlayerData.Instance.IsStarted == false)
        {
            blackPanel.SetActive(true);
        }
        else
        {
            blackPanel.SetActive(false);
        }
    }
    private async void OnMouseDown()
    {
        if (PlayerData.Instance.IsStarted == false) return; // すでにクリックされている場合は何もしない
        await SceneChanger.Instance.ChangeScene("Custom", 2.0f, 1.0f);
    }
}
