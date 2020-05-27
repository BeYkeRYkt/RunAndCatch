public interface IPerk : IGameStateListener
{
    bool shouldBeActivated();

    void OnPerkActivated();
}
