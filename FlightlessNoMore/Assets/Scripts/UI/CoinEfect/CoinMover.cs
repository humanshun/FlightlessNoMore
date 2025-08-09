using UnityEngine;
using DG.Tweening;
using System;

public class CoinMover : MonoBehaviour
{
    private Transform target;
    private float moveDuration;
    private Action<CoinMover> onComplete;
    private Sequence seq;

    public bool IsCompleted { get; private set; } = false;

    public void Init(Transform targetTransform, float duration, Action<CoinMover> onCompleteCallback)
    {
        target = targetTransform;
        moveDuration = duration;
        onComplete = onCompleteCallback;

        PlaySequence();
    }

    private void PlaySequence()
    {
        seq = DOTween.Sequence();

        transform.localScale = Vector3.zero;
        seq.Append(transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.2f);

        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 reverseOffset = transform.position - direction * 200.0f;

        seq.Append(transform.DOMove(reverseOffset, 0.3f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOMove(target.position, moveDuration).SetEase(Ease.InOutQuad));

        seq.OnComplete(() =>
        {
            IsCompleted = true;
            onComplete?.Invoke(this);
            Destroy(gameObject);
            AudioManager.Instance.PlaySFX("SE_Coins");
        });
    }

    public void SkipToTarget()
    {
        if (IsCompleted) return;

        seq?.Kill(); // DOTweenのシーケンスを中断
        transform.position = target.position;
        IsCompleted = true;
        onComplete?.Invoke(this);
        Destroy(gameObject);
    }
}
