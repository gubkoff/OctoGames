using OctoGames.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace OctoGames.App.Features.Popups.ConfirmPopup
{
    public sealed class ConfirmPopup : PopupBaseView
    {
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _bodyText;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Text _confirmButtonLabel;
        [SerializeField] private Text _cancelButtonLabel;

        internal Text TitleText => _titleText;
        internal Text BodyText => _bodyText;
        internal Button ConfirmButton => _confirmButton;
        internal Button CancelButton => _cancelButton;
        internal Text ConfirmButtonLabel => _confirmButtonLabel;
        internal Text CancelButtonLabel => _cancelButtonLabel;
    }
}
