using UnityEngine;

public class ItemBase : MonoBehaviour
{
    protected virtual void Start()
    {
        gameObject.SetActive(true);
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
    }
}
