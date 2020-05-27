using System.Collections.Generic;
using UnityEngine;

/**
 * Class for GameRoom
 */
public class GameRoomManager : MonoBehaviour
{
    // DEBUG ONLY
    public bool isSessionRunning;

    private List<IGameRoomListener> mListeners = new List<IGameRoomListener>();

    public static GameRoomManager Instance { get; private set; }

    void Awake()
    {
        CreateSingleton();
        //canvasObject = GameObject.Find("Canvas");
    }

    protected void CreateSingleton()
    {
        Instance = this;
    }

    public void RegisterListener(IGameRoomListener listener)
    {
        if (!mListeners.Contains(listener))
        {
            mListeners.Add(listener);
        }
    }

    public void UnregisterListener(IGameRoomListener listener)
    {
        if (mListeners.Contains(listener))
        {
            mListeners.Remove(listener);
        }
    }

    public void StartGame()
    {
        isSessionRunning = true;

        // call game room listeners
        foreach (IGameRoomListener listener in mListeners)
        {
            listener.OnGameRoomStarted();
        }
    }

    public void StopGame()
    {
        isSessionRunning = false;

        // call game room listeners
        foreach (IGameRoomListener listener in mListeners)
        {
            listener.OnGameRoomStarted();
        }
    }

    // Player events
    public void OnPlayerDeath(EntityPlayer player)
    {
        // call game state listeners
        foreach (IGameRoomListener listener in mListeners)
        {
            listener.OnPlayerDeath(player);
        }
    }
}
