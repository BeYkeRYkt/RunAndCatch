
public interface IGameRoomListener
{
    void OnGameRoomStarted();

    void OnGameRoomStopped();

    // Player events
    void OnPlayerDeath(EntityPlayer player);
}
