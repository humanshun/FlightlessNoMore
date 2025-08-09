using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public enum TitleState
    {
        Title,
        Setting,
        Play
    }
    [SerializeField] private GameObject titleBordPrefab;
    [SerializeField] private GameObject settingBordPrefab;
    [SerializeField] private GameObject playBordPrefab;
    [SerializeField] private Transform spawnPoint;
    private TitleState currentState = TitleState.Title;
    private GameObject currentBord;
    private TitleManager titleManager;
    private MenuManager menuManager;
    private TitleButton titleButton;
    private bool isChanging = false;
    private string clickSound = "SE_ButtonLow";

    void Awake()
    {
        titleManager = this;
    }

    async void Start()
    {
        await UniTask.Delay(16000);

        currentBord = Instantiate(titleBordPrefab, spawnPoint.position, Quaternion.identity);
        menuManager = currentBord.GetComponent<MenuManager>();
        menuManager.Setting(titleManager);
        currentState = TitleState.Title;
        isChanging = true;

        await UniTask.Delay(4000);

        isChanging = false;
    }

    public async void ChangeState(TitleState newState)
    {
        if (isChanging) return; // 連打防止
        isChanging = true;

        switch (currentState)
        {
            case TitleState.Title:
                menuManager.End();
                break;

            case TitleState.Setting:
                menuManager.End();
                break;

            case TitleState.Play:
                menuManager.End();
                break;
        }

        currentState = newState;

        switch (currentState)
        {
            case TitleState.Title:
                currentBord = Instantiate(titleBordPrefab, spawnPoint.position, Quaternion.identity);
                menuManager = currentBord.GetComponent<MenuManager>();
                menuManager.Setting(titleManager);
                AudioManager.Instance.PlaySFX(clickSound);
                break;

            case TitleState.Setting:
                currentBord = Instantiate(settingBordPrefab, spawnPoint.position, Quaternion.identity);
                menuManager = currentBord.GetComponent<MenuManager>();
                menuManager.Setting(titleManager);
                AudioManager.Instance.PlaySFX(clickSound);
                break;

            case TitleState.Play:
                currentBord = Instantiate(playBordPrefab, spawnPoint.position, Quaternion.identity);
                menuManager = currentBord.GetComponent<MenuManager>();
                menuManager.Setting(titleManager);
                AudioManager.Instance.PlaySFX(clickSound);
                break;
        }
        menuManager = currentBord.GetComponent<MenuManager>();

        await UniTask.Delay(4000);
        isChanging = false;
    }

}
