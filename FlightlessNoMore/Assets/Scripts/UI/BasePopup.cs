using UnityEngine;

public abstract class BasePopup : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPopup(this);
        }
    }

    protected virtual void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterPopup(this);
        }
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}
