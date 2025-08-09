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
