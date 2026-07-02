using OctoGames.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace OctoGames.App.Features.Popups.EntityDetailPopup
{
    public sealed class EntityDetailPopup : PopupBaseView
    {
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _typeText;
        [SerializeField] private Text _stateText;
        [SerializeField] private Button _disableButton;
        [SerializeField] private Button _enableButton;
        [SerializeField] private Button _completeButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _closeButton;

        internal Text TitleText => _titleText;
        internal Text TypeText => _typeText;
        internal Text StateText => _stateText;
        internal Button DisableButton => _disableButton;
        internal Button EnableButton => _enableButton;
        internal Button CompleteButton => _completeButton;
        internal Button DeleteButton => _deleteButton;
        internal Button CloseButton => _closeButton;
    }
}
