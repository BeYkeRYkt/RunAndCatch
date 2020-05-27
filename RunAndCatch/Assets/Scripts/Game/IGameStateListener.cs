
public interface IGameStateListener
{
    void OnGameStarted();

    void OnGameStopped();

    // TODO: Remove this in future
    void OnGamePaused();

    void OnGameUnpaused();

    void OnGameOver();
}
