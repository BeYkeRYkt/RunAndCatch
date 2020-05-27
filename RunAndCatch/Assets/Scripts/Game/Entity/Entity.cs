
using Photon.Pun;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int entityId;

    // Physics
    private Collider collider;
    private bool m_isCollided;

    // Start is called before the first frame update
    public virtual void Start()
    {
        collider = GetComponent<Collider>();

        EntityManager manager = FindObjectOfType<EntityManager>();
        if (manager != null)
        {
            manager.AddEntity(this);
        }
    }

    public virtual void OnDestroy()
    {
        EntityManager manager = FindObjectOfType<EntityManager>();
        if (manager != null)
        {
            manager.RemoveEntity(this);
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if(transform.position.y < -30)
        {
            Debug.Log(gameObject.name + ": DA BLYA");
            KillEntity();
        }
    }

    public bool IsGrounded()
    {
        RaycastHit hit;
        float distance = collider.bounds.extents.y + 0.1f;
        if (Physics.SphereCast(collider.bounds.center, 0.2f, Vector3.down, out hit, distance))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, 2.5f);
            return true;
        }
        return false;
    }

    public void KillEntity()
    {
        NetworkEntity sync = GetComponent<NetworkEntity>();
        if(sync != null)
        {
            sync.KillEntity();
        }
        OnEntityDeath();
        Debug.Log(gameObject.name + ": HOLY SHIT! I'M DEAD! Not big surprise.");
        Destroy(gameObject);
    }

    public void KillEntityRPC()
    {
        OnEntityDeath();
        Debug.Log(gameObject.name + ": HOLY SHIT! I'M DEAD! Not big surprise.");
        Destroy(gameObject);
    }

    public virtual void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public virtual Vector3 GetPosition()
    {
        return transform.position;
    }

    public virtual void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation.normalized;
    }

    public virtual Quaternion GetRotation()
    {
        return transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_isCollided = true;
        Debug.Log(gameObject.name + ": damn: " + m_isCollided);
    }

    private void OnCollisionExit(Collision collision)
    {
        m_isCollided = false;
        Debug.Log(gameObject.name + ": damn: " + m_isCollided);
    }

    public bool IsCollided()
    {
        return m_isCollided;
    }

    public virtual void OnEntityDeath()
    {
    }
}
