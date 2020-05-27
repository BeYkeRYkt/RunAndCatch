using UnityEngine;

public class EntityPlayer : NameableEntity
{
    private PlayerInfo info;

    public PlayerInfo Info { get => info; set => info = value; }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Info = PlayerInfo.DEFAULT;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void OnEntityDeath()
    {
        NetworkEntityPlayer sync = GetComponent<NetworkEntityPlayer>();
        if(sync != null)
        {
            sync.OnDeathEntity();
        }
    }
}
