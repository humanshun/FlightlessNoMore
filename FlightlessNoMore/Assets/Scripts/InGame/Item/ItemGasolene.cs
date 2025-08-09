using UnityEngine;

public class ItemGasolene : ItemBase
{
    [SerializeField] private float recoverAmount = 3.0f; // 回復する噴射時間（秒数）
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            PlayerController2 player = collider.GetComponentInParent<PlayerController2>();
            if (player != null)
            {
                // ロケット時間回復
                player.RecoverRocketTime(recoverAmount);

                // 回復アイテムのSE
                AudioManager.Instance.PlaySFX("SE_Heal2");

                // TODOエフェクトを出したいならここでInstantiate

                // 取ったら消す
                gameObject.SetActive(false);
            }
        }
    }
}
