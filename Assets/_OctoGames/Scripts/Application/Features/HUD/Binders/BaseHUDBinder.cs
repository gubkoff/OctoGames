using Cysharp.Threading.Tasks;
using OctoGames.App.Features.Entities;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace OctoGames.App.Features.HUD
{
    public sealed class BaseHUDBinder : MonoBehaviour
    {
        private BaseHUD _view;
        private CompositeDisposable _disposables;
        private IObjectResolver _resolver;
        private BaseHUDViewModel _viewModel;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        private void Awake()
        {
            _view = GetComponent<BaseHUD>();
        }

        private void Start()
        {
            _disposables = new CompositeDisposable();
            _viewModel = _resolver.Resolve<BaseHUDViewModel>();
            _viewModel.Activate();
            BindView(_view, _viewModel, _disposables);
        }

        private void OnDestroy()
        {
            _viewModel?.Dispose();
            _disposables?.Dispose();
        }

        private void BindView(BaseHUD view, BaseHUDViewModel viewModel, CompositeDisposable disposables)
        {
            var ct = destroyCancellationToken;

            viewModel.ActiveCount
                .Subscribe(count =>
                {
                    if (view.ActiveCountText != null)
                        view.ActiveCountText.text = $"Active: {count}";
                })
                .AddTo(disposables);

            BindActionButton(view.AddEnemyButton, () => viewModel.AddEntityAsync(GameplayEntityType.Enemy, ct), disposables);
            BindActionButton(view.AddInteractableButton, () => viewModel.AddEntityAsync(GameplayEntityType.Interactable, ct), disposables);
            BindActionButton(view.AddStoryActorButton, () => viewModel.AddEntityAsync(GameplayEntityType.StoryActor, ct), disposables);
            BindActionButton(view.RestartButton, () => viewModel.RestartAsync(ct), disposables);
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
