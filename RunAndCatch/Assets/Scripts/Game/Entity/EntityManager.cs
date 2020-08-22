using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    private int lastId = 0;
    public List<Entity> entities = new List<Entity>();
    public List<EntityPlayer> playerEntities = new List<EntityPlayer>();

    public static EntityManager Instance { get; private set; }


    void Awake()
    {
        CreateSingleton();
    }

    protected void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public void AddEntity(Entity entity)
    {
        entity.entityId = lastId;
        entities.Add(entity);
        lastId++;
        if(entity is EntityPlayer)
        {
            EntityPlayer player = (EntityPlayer) entity;
            playerEntities.Add(player);
        }
    }

    public void RemoveEntity(Entity entity)
    {
        entities.Remove(entity);
        if (entity is EntityPlayer)
        {
            EntityPlayer player = (EntityPlayer)entity;
            playerEntities.Remove(player);
        }
    }

    public Entity getEntityById(int entityId)
    {
        Entity target = null;
        foreach (Entity entity in entities)
        {
            if (entity.entityId == entityId)
            {
                target = entity;
                break;
            }
        }
        return target;
    }

    public NameableEntity GetEntityByName(string name)
    {
        NameableEntity target = null;
        foreach (Entity entity in entities)
        {
            if (entity is NameableEntity)
            {
                NameableEntity nEntity = (NameableEntity) entity;
                if (nEntity.GetDisplayName() == name)
                {
                    target = nEntity;
                    break;
                }
            }
        }
        return target;
    }

    public List<EntityPlayer> GetPlayerEntities()
    {
        return playerEntities;
    }

    public EntityPlayer getPlayerEntityByName(string name)
    {
        EntityPlayer target = null;
        foreach (EntityPlayer entity in entities)
        {
            if (entity.GetDisplayName() == name)
            {
                target = entity;
                break;
            }
        }
        return target;
    }
}
