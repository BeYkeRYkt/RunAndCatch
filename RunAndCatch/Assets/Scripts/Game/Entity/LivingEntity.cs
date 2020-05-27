using UnityEngine;

public class LivingEntity : PhysicalEntity
{
    private int health;
    private int maxHealth;

    public void SetMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
        if(maxHealth < GetHealth())
        {
            SetHealth(maxHealth);
        }
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public int GetHealth()
    {
        return health;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        SetMaxHealth(20);
        SetHealth(20);
    }

    public override void Update()
    {
        base.Update();

        if(GetHealth() <= 0)
        {
            Debug.Log("OH SHIT! I'M DEAD! Not big surprise.");
            KillEntity();
        }
    }
}
