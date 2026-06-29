namespace OctoGames.App.Features.Popups
{
    public readonly struct PopupShowOptions
    {
        public PopupShowOptions(PopupShowPolicy policy = PopupShowPolicy.Queue)
        {
            Policy = policy;
        }

        public PopupShowPolicy Policy { get; }
    }
}
