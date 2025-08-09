using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public PlayerData playerData; // プレイヤーデータの参照
    public ShopData shopData; // ショップアイテムのデータ配列
    // public Button[] buyButtons; // 購入ボタンの配列
    // public TextMeshProUGUI[] itemNameText; // アイテム表示用のテキスト

    // void Start()
    // {
    //     for (int i = 0; i < shopItems.Length; i++)
    //     {
    //         if (i < buyButtons.Length && buyButtons[i] != null)
    //         {
    //             int index = i; // ローカル変数にインデックスを保存
    //             buyButtons[i].onClick.AddListener(() => BuyItem(shopItems[index], buyButtons[index]));
    //             buyButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = shopItems[index].partCost + "$";
    //             itemNameText[i].text = shopItems[index].partName;
    //         }
    //     }
    // }

    // void BuyItem(PartData item, Button button)
    // {
    //     if (playerData.playerCoins >= item.partCost)
    //     {
    //         playerData.playerCoins -= item.partCost;
    //         Debug.Log(playerData.playerCoins);
    //         button.interactable = false; // ボタンを無効にする
    //     }
    //     else
    //     {
    //         Debug.Log("コインが不足しています。");
    //     }
    // }
}