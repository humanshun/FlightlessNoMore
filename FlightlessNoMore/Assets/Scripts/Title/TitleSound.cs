using UnityEngine;
using Cysharp.Threading.Tasks;

public class TitleSound : MonoBehaviour
{
    [SerializeField] private string jumpsound = "SE_Jump";
    [SerializeField] private string walksound = "SE_Walk";
    [SerializeField] private int delay;

    public async void PlayJumpSound()
    {
        AudioManager.Instance.PlaySFX(jumpsound);
        await UniTask.Delay(delay);
        AudioManager.Instance.PlaySFX(jumpsound);
    }

    public void PlayWalkSound(float speed)
    {
        AudioManager.Instance.PlaySFX(walksound, speed);
    }

    public void StapSound()
    {
        AudioManager.Instance.StopSFX(walksound);
    }
}
