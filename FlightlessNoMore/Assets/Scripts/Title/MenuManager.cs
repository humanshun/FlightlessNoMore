using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] string bord;
    [SerializeField] CharacterBird characterBird1;
    [SerializeField] CharacterBird characterBird2;
    [SerializeField] GameObject[] buttons;
    private void Start()
    {
        characterBird1.stopPosition = -2;
        characterBird2.stopPosition = 2;
    }

    public void Setting(TitleManager titleManager)
    {
        foreach (var button in buttons)
        {
            if (button == null) continue;
            TitleButton titleButton = button.GetComponent<TitleButton>();
            if (titleButton != null)
            {
                titleButton.SetTitleManager(titleManager);
            }
        }
    }

    public async void End()
    {
        characterBird1.stopPosition = 14;
        characterBird2.stopPosition = 16;

        await UniTask.Delay(4000);

        if (this != null && gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
