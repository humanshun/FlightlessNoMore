using UnityEngine;

public class ItemCoin : ItemBase
{
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            var inGameUI = GameManager.Instance?.InGameUI;
            if (inGameUI != null)
            {
                inGameUI.AddCollectedCoins(100);
            }
            //コインの音を再生
            AudioManager.Instance.PlaySFX("SE_Get");

            // 取ったら消す
            gameObject.SetActive(false);
        }
    }
}
