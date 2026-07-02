using OctoGames.Popups;
using R3;
using UnityEngine.UI;
using VContainer;

namespace OctoGames.App.Features.Popups.ClassicConfirmPopup
{
    public sealed class ClassicConfirmPopupBinder : PopupBinderBase<ClassicConfirmPopup>
    {
        public override IPopupViewModel ResolveViewModel(IObjectResolver resolver) =>
            resolver.Resolve<ConfirmPopupViewModel>();

        protected override void BindView(
            ClassicConfirmPopup popup,
            IPopupViewModel viewModel,
            CompositeDisposable disposables)
        {
            var confirmViewModel = (ConfirmPopupViewModel)viewModel;

            confirmViewModel.Title
                .Subscribe(value =>
                {
                    if (popup.TitleText != null)
                        popup.TitleText.text = value;
                })
                .AddTo(disposables);

            confirmViewModel.Body
                .Subscribe(value =>
                {
                    if (popup.BodyText != null)
                        popup.BodyText.text = value;
                })
                .AddTo(disposables);

            confirmViewModel.ConfirmLabel
                .Subscribe(value =>
                {
                    if (popup.ConfirmButtonLabel != null)
                        popup.ConfirmButtonLabel.text = value;
                })
                .AddTo(disposables);

            confirmViewModel.CancelLabel
                .Subscribe(value =>
                {
                    if (popup.CancelButtonLabel != null)
                        popup.CancelButtonLabel.text = value;
                })
                .AddTo(disposables);

            if (popup.ConfirmButton != null)
            {
                popup.ConfirmButton.OnClickAsObservable()
                    .Subscribe(_ => confirmViewModel.Confirm())
                    .AddTo(disposables);
            }

            if (popup.CancelButton != null)
            {
                popup.CancelButton.OnClickAsObservable()
                    .Subscribe(_ => confirmViewModel.Cancel())
                    .AddTo(disposables);
            }
        }
    }
}
