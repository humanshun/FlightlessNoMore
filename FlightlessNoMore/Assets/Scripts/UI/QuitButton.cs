using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    [SerializeField] private Button quitButton;
    void Start()
    {
        quitButton.onClick.AddListener(QuitButtonOnClick);
    }
    void QuitButtonOnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Appliation.Quit();
#endif
    }
}
