using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDetector : MonoBehaviour
{
    public void DetectButton()
    {
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;

        if (selectedButton != null)
        {
            Debug.Log($"押されたボタン: {selectedButton.name}");
        }
        else
        {
            Debug.Log("⚠ ボタンが選択されていません！");
        }
    }
}
