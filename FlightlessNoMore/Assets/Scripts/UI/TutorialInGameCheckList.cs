using UnityEngine;
using UnityEngine.UI;

public class TutorialInGameCheckList : MonoBehaviour
{
    [SerializeField] private GameObject checkListPanel;
    [SerializeField] private Toggle checkList1;
    [SerializeField] private Toggle checkList2;
    [SerializeField] private Toggle checkList3;
    [SerializeField] private Toggle checkList4;

    public void CheckList()
    {
        checkListPanel.SetActive(true);
        if (PlayerData.Instance.playerCoins > 10)
        {
            checkList1.isOn = true;
        }
        if (GameManager.Instance.isBuyPart)
        {
            checkList2.isOn = true;
        }
        if (GameManager.Instance.isChangePart)
        {
            checkList3.isOn = true;
        }
        if (GameManager.Instance.isClearTutorial)
        {
            checkList4.isOn = true;
        }
    }
}
