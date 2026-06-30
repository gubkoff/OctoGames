using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace OctoGames.App.Features.Popups
{
    public abstract class PopupBinderBase<TView> : MonoBehaviour, IPopupBinder<TView>
        where TView : PopupBaseView
    {
        public abstract IPopupViewModel ResolveViewModel(IObjectResolver resolver);

        public void Bind(TView view, IPopupViewModel viewModel, CompositeDisposable disposables)
        {
            BindCommon(view, viewModel, disposables);
            BindView(view, viewModel, disposables);
        }

        protected virtual void BindCommon(TView view, IPopupViewModel viewModel, CompositeDisposable disposables)
        {
            BindDimmerButton(view, viewModel, disposables);
        }

        protected abstract void BindView(TView view, IPopupViewModel viewModel, CompositeDisposable disposables);

        protected static void BindDimmerButton(
            PopupBaseView view,
            IPopupViewModel viewModel,
            CompositeDisposable disposables)
        {
            if (!view.CloseOnDimmerClick || view.DimmerButton == null)
                return;

            view.DimmerButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (view.Phase == PopupPhase.Opened)
                        viewModel.RequestClose();
                })
                .AddTo(disposables);
        }

        protected static void BindCloseButton(
            Button button,
            IPopupViewModel viewModel,
            CompositeDisposable disposables)
        {
            if (button == null)
                return;

            button.OnClickAsObservable()
                .Subscribe(_ => viewModel.RequestClose())
                .AddTo(disposables);
        }
    }
}
