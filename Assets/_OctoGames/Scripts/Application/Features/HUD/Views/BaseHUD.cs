using UnityEngine;
using UnityEngine.UI;

namespace OctoGames.App.Features.HUD
{
    public sealed class BaseHUD : MonoBehaviour
    {
        [SerializeField] private Text _activeCountText;
        [SerializeField] private Button _addEnemyButton;
        [SerializeField] private Button _addInteractableButton;
        [SerializeField] private Button _addStoryActorButton;
        [SerializeField] private Button _restartButton;

        internal Text ActiveCountText => _activeCountText;
        internal Button AddEnemyButton => _addEnemyButton;
        internal Button AddInteractableButton => _addInteractableButton;
        internal Button AddStoryActorButton => _addStoryActorButton;
        internal Button RestartButton => _restartButton;
    }
}
