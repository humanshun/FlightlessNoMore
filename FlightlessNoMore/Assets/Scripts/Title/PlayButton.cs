using System;
using Ricimi;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public enum ButtonType
    {
        NewGame,
        Continue
    }
    [SerializeField] private ButtonType buttonType;

    private void Start()
    {
        // Continueボタンの場合、セーブデータがなければ非表示にする
        if (buttonType == ButtonType.Continue)
        {
            // SaveManagerが存在しない場合は何もしない
            if (SaveManager.Instance == null)
                return;
            // セーブデータファイルが存在しなければ非表示
            if (!System.IO.File.Exists(UnityEngine.Application.persistentDataPath + "/save.json"))
            {
                gameObject.SetActive(false);
            }
        }
    }

    private async void OnMouseDown()
    {
        switch (buttonType)
        {
            case ButtonType.NewGame:
                PlayerData.Instance.ResetPlayerData();
                await SceneChanger.Instance.ChangeScene("Custom", 1.0f, 1.0f);
                break;
            case ButtonType.Continue:
                Transition.LoadLevel("Custom", 2.0f, Color.black);
                break;
        }
    }
}
