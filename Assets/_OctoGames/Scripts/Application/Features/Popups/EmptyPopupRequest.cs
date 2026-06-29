namespace OctoGames.App.Features.Popups
{
    public sealed class EmptyPopupRequest : IPopupRequest
    {
        public static readonly EmptyPopupRequest Instance = new();

        private EmptyPopupRequest() { }
    }
}
