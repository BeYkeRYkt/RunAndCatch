using System.Collections.Generic;
using UnityEngine;

/**
 * Main game logic 
 */
public class GameManager : MonoBehaviour
{
    // DEBUG ONLY
    public bool isOnline;
    public bool isSessionRunning;
    public bool isPaused;
    public bool isPlayerDead;
    public bool isGameOver;

    public GameObject playerObject;
    public GameObject cameraObject;
    //public GameObject canvasObject;

    private List<IGameStateListener> mListeners = new List<IGameStateListener>();

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        CreateSingleton();
        //canvasObject = GameObject.Find("Canvas");
    }

    protected void CreateSingleton()
    {
        Instance = this;
    }

    void Start()
    {
        StartGame();
    }

    public void RegisterListener(IGameStateListener listener)
    {
        if (!mListeners.Contains(listener))
        {
            mListeners.Add(listener);
        }
    }

    public void UnregisterListener(IGameStateListener listener)
    {
        if (mListeners.Contains(listener))
        {
            mListeners.Remove(listener);
        }
    }

    public void StartGame()
    {
        isSessionRunning = true;

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGameStarted();
        }
    }

    public void StopGame()
    {
        isSessionRunning = false;

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGameStopped();
        }
    }

    public void PauseGame()
    {
        if (!isSessionRunning)
        {
            return;
        }

        isPaused = true;

        //canvasObject.SetActive(true);

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGamePaused();
        }
    }

    public void UnpauseGame()
    {
        isPaused = false;

        //canvasObject.SetActive(false);

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGameUnpaused();
        }
    }

    public void GameOver()
    {
        isGameOver = true;

        // call game state listeners
        foreach (IGameStateListener listener in mListeners)
        {
            listener.OnGameOver();
        }

        // show game over UI
        if (isSessionRunning)
        {
            //UIHome.instance.ShowGameOver();
        }
    }

    // Player events
    public void onPlayerDeath()
    {
        isPlayerDead = true;
        if(cameraObject != null)
        {
            cameraObject.transform.parent.parent = null;
            CameraFollower flwr = cameraObject.transform.parent.GetComponent<CameraFollower>();
            if (flwr != null)
            {
                flwr.ResetPos();
            }
        }
        PauseGame();
    }

    private void Update()
    {
        // handle key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                UnpauseGame();
            }
        }
    }

}
