using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace OctoGames.App.Features.Popups
{
    public abstract class PopupBaseView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private PopupAnimation _animation;
        [SerializeField] private bool _closeOnBackdropTap;
        [SerializeField] private Button _dimmerButton;

        private IPopupViewModel _viewModel;
        private PopupPhase _phase = PopupPhase.Closed;

        public virtual System.Type PopupType => GetType();

        public PopupPhase Phase => _phase;

        public bool IsInteractive => _phase == PopupPhase.Opened;

        public bool CloseOnBackdropTap => _closeOnBackdropTap;

        protected virtual void Awake()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            if (_animation == null)
                _animation = GetComponent<PopupAnimation>();
        }

        internal void RegisterViewModel(IPopupViewModel viewModel) => _viewModel = viewModel;

        internal void SetPhase(PopupPhase phase) => _phase = phase;

        internal UniTask PlayOpenAsync(CancellationToken ct)
        {
            return _animation.PlayOpenAsync(_canvasGroup, ct);
        }

        internal UniTask PlayCloseAsync(CancellationToken ct)
        {
            return _animation.PlayCloseAsync(_canvasGroup, ct);
        }

        public void OnDimmerClick(CompositeDisposable disposable)
        {
            if (!_closeOnBackdropTap || _dimmerButton == null)
                return;

            _dimmerButton.OnClickAsObservable()
                .Subscribe(_ => RequestClose())
                .AddTo(disposable);
        }

        public void RequestClose()
        {
            if (_phase != PopupPhase.Opened)
                return;

            _viewModel?.RequestClose();
        }
    }
}
