using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    private int currentCoin = 0;

    private void Start()
    {
        currentCoin = PlayerData.Instance.playerCoins;
        coinText.text = currentCoin.ToString();
        PlayerData.OnCoinsChanged += UpdateCoinText;
    }

    private void OnDestroy()
    {
        PlayerData.OnCoinsChanged -= UpdateCoinText;
    }

    private void UpdateCoinText(int newCoin)
    {
        currentCoin = newCoin;
        coinText.text = currentCoin.ToString();
    }


}
