using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.Popups;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace OctoGames.App.Features.Popups.EntityDetailPopup
{
    public sealed class EntityDetailPopupBinder : PopupBinderBase<EntityDetailPopup>
    {
        public override IPopupViewModel ResolveViewModel(IObjectResolver resolver) =>
            resolver.Resolve<EntityDetailPopupViewModel>();

        protected override void BindView(
            EntityDetailPopup popup,
            IPopupViewModel viewModel,
            CompositeDisposable disposables)
        {
            var detailViewModel = (EntityDetailPopupViewModel)viewModel;
            var ct = popup.destroyCancellationToken;

            detailViewModel.Title
                .Subscribe(value =>
                {
                    if (popup.TitleText != null)
                        popup.TitleText.text = value;
                })
                .AddTo(disposables);

            detailViewModel.TypeLabel
                .Subscribe(value =>
                {
                    if (popup.TypeText != null)
                        popup.TypeText.text = $"Type: {value}";
                })
                .AddTo(disposables);

            detailViewModel.StateLabel
                .Subscribe(value =>
                {
                    if (popup.StateText != null)
                        popup.StateText.text = $"State: {value}";
                })
                .AddTo(disposables);

            BindButtonVisibility(popup.DisableButton, detailViewModel.ShowDisable, disposables);
            BindButtonVisibility(popup.EnableButton, detailViewModel.ShowEnable, disposables);
            BindButtonVisibility(popup.CompleteButton, detailViewModel.ShowComplete, disposables);
            BindButtonVisibility(popup.DeleteButton, detailViewModel.ShowDelete, disposables);

            BindActionButton(popup.DisableButton, () => detailViewModel.DisableAsync(ct), disposables);
            BindActionButton(popup.EnableButton, () => detailViewModel.EnableAsync(ct), disposables);
            BindActionButton(popup.CompleteButton, () => detailViewModel.CompleteAsync(ct), disposables);
            BindActionButton(popup.DeleteButton, () =>
            {
                detailViewModel.RequestDelete();
                return UniTask.CompletedTask;
            }, disposables);
            BindCloseButton(popup.CloseButton, detailViewModel, disposables);
        }

        private static void BindButtonVisibility(
            Button button,
            ReactiveProperty<bool> visibility,
            CompositeDisposable disposables)
        {
            if (button == null)
                return;

            visibility
                .Subscribe(value => button.gameObject.SetActive(value))
                .AddTo(disposables);
        }

        private static void BindActionButton(
            Button button,
            System.Func<UniTask> action,
            CompositeDisposable disposables)
        {
            if (button == null)
                return;

            button.OnClickAsObservable()
                .Subscribe(_ => action().Forget())
                .AddTo(disposables);
        }
    }
}
