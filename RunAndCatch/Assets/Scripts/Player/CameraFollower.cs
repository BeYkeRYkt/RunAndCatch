﻿
using UnityEngine;

public class CameraFollower : MonoBehaviour, IGameStateListener
{
    public GameObject defaultPos; 
    public GameObject target;
    public Vector3 target_Offset;

    public void OnGameOver()
    {
        //GameObject resp = GameObject.Find("RespManager");
        //target = resp.transform;
        //target_Offset = transform.position - target.position;
        ResetPos();
    }

    public void OnGamePaused()
    {
    }

    public void OnGameStarted()
    {
        //target = GameObject.FindGameObjectWithTag("Player").transform;
        //target_Offset = transform.position - target.position;
    }

    public void OnGameStopped()
    {
    }

    public void OnGameUnpaused()
    {
    }

    // Start is called before the first frame update
    void Awake()
    {
        ClientGameManager gm = ClientGameManager.Instance;
        gm.RegisterListener(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position + target_Offset, 0.1f);
        }
    }

    public void ResetPos()
    {
        //target = defaultPos;
        target = null;
        transform.position = defaultPos.transform.position;
        transform.rotation = defaultPos.transform.rotation;
        //target_Offset = new Vector3(0, 0, 0);
    }
}

