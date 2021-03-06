﻿using UnityEngine;

public class EntityPlayer : NameableEntity
{
    private PlayerInfo info = PlayerInfo.DEFAULT;
    public PlayerRole playerRole = PlayerRole.VICTIM;

    public PlayerInfo Info { get => info; set => info = value; }

    public PlayerRole GetPlayerRole()
    {
        return playerRole;
    }

    public void SetPlayerRole(PlayerRole role)
    {
        playerRole = role;
    }

    public override void OnEntityDeath()
    {
        base.OnEntityDeath();
        NetworkEntityPlayer sync = GetComponent<NetworkEntityPlayer>();
        if(sync != null)
        {
            sync.OnEntityDeath();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(playerRole == PlayerRole.HUNTER)
        {
            Debug.Log(other.gameObject.name + " : " + other.tag);

            if (other.tag == "Player")
            {
                EntityPlayer player = other.gameObject.GetComponent<EntityPlayer>();
                player.KillEntity();
            }
        }
    }
}
