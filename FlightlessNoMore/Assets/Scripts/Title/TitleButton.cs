using UnityEngine;

public class TitleButton : MonoBehaviour
{
    [HideInInspector]
    public TitleManager titleManager;  // 外部から直接アサイン不可
    [SerializeField] private TitleManager.TitleState targetState;  // 遷移先

    public void SetTitleManager(TitleManager manager)
    {
        titleManager = manager;
    }

    void OnMouseDown()
    {
        // 左クリックされたときに呼ばれる
        if (titleManager != null)
        {
            titleManager.ChangeState(targetState);
        }
    }
}
