using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System;

public class CharacterMover : MonoBehaviour
{
    [System.Serializable]
    public class MoveStep
    {
        public AnimationClip clip;               // 再生するアニメーションクリップ
        public Vector3 startPosition;            // 開始位置
        public Vector3 endPosition;              // 終了位置
        public Vector3 scale = new Vector3(1, 1, 1);
        public float moveDuration = 1.2f;        // 移動時間
        public float waitBeforeMove = 0.3f;      // アニメーション前の待機
        public float waitAfterMove = 0.5f;       // 移動後の待機
        // ステップ実行時に呼び出されるイベント。Inspectorから任意の処理を追加可能。
        public UnityEvent onStepAction;
    }

    public Animator animator;
    public List<MoveStep> steps = new();

    void Start()
    {
        PlaySequenceAsync(transform).Forget();
    }

    public async UniTask PlaySequenceAsync(Transform characterTransform)
    {
        foreach (var step in steps)
        {
            characterTransform.position = step.startPosition;

            await UniTask.Delay(TimeSpan.FromSeconds(step.waitBeforeMove));

            if (animator != null && step.clip != null)
            {
                animator.Play(step.clip.name);
            }

            step.onStepAction?.Invoke();

            characterTransform.localScale = step.scale;

            await characterTransform.DOMove(step.endPosition, step.moveDuration)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();

            await UniTask.Delay(TimeSpan.FromSeconds(step.waitAfterMove));
        }
    }

    public void AnimStop()
    {
        animator.speed = 0;
    }
}
