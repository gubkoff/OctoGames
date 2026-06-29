using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OctoGames.App.Features.Popups
{
    public sealed class FadePopupAnimation : PopupAnimation
    {
        [SerializeField] private float _duration = 0.2f;

        public override async UniTask PlayOpenAsync(CanvasGroup group, CancellationToken ct)
        {
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = true;
            await AnimateAlpha(group, 0f, 1f, ct);
            group.interactable = true;
        }

        public override async UniTask PlayCloseAsync(CanvasGroup group, CancellationToken ct)
        {
            group.interactable = false;
            await AnimateAlpha(group, group.alpha, 0f, ct);
            group.blocksRaycasts = false;
        }

        private async UniTask AnimateAlpha(CanvasGroup group, float from, float to, CancellationToken ct)
        {
            if (_duration <= 0f)
            {
                group.alpha = to;
                return;
            }

            var elapsed = 0f;
            while (elapsed < _duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / _duration);
                group.alpha = Mathf.Lerp(from, to, t);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            group.alpha = to;
        }
    }
}
