using R3;
using VContainer;

namespace OctoGames.Popups
{
    public interface IPopupBinder<in TView>
        where TView : PopupBaseView
    {
        IPopupViewModel ResolveViewModel(IObjectResolver resolver);

        void Bind(TView view, IPopupViewModel viewModel, CompositeDisposable disposables);
    }
}
