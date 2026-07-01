using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OctoGames.Popups
{
    public abstract class PopupAnimation : MonoBehaviour
    {
        public abstract UniTask PlayOpenAsync(CanvasGroup group, CancellationToken ct);

        public abstract UniTask PlayCloseAsync(CanvasGroup group, CancellationToken ct);
    }
}
