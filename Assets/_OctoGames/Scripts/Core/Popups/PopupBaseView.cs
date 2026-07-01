using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OctoGames.Popups
{
    public abstract class PopupBaseView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private PopupAnimation _animation;
        [SerializeField] private bool _closeOnDimmerClick;
        [SerializeField] private Button _dimmerButton;

        private PopupPhase _phase = PopupPhase.Closed;

        public virtual System.Type PopupType => GetType();

        public PopupPhase Phase => _phase;

        public bool IsInteractive => _phase == PopupPhase.Opened;

        public bool CloseOnDimmerClick => _closeOnDimmerClick;

        internal Button DimmerButton => _dimmerButton;

        protected virtual void Awake()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            if (_animation == null)
                _animation = GetComponent<PopupAnimation>();
        }

        internal void SetPhase(PopupPhase phase) => _phase = phase;

        internal UniTask PlayOpenAsync(CancellationToken ct)
        {
            return _animation.PlayOpenAsync(_canvasGroup, ct);
        }

        internal UniTask PlayCloseAsync(CancellationToken ct)
        {
            return _animation.PlayCloseAsync(_canvasGroup, ct);
        }
    }
}
