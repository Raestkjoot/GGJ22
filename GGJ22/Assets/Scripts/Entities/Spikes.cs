using UnityEngine;

public class Spikes : MonoBehaviour
{
    public enum Type
    {
        Static,
        Periodic
    }
    public void Enable()
    {
        active = true;
    }
    public void Disable() 
    {
        active = false;
    }
    public void SetType(Type type) 
    {
        myType = type;
        timer = 0.0f;

        if (type == Type.Static)
            Enable();
    }
    public void SetPeriodic(float interval) 
    {
        float oldInterval = periodicInterval;
        periodicInterval = interval;
        timer = MathUtils.Map(timer, 0.0f, oldInterval, 0.0f, interval);
    }

    private void Start()
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        entity = entityManager.Create(transform.GetInstanceID());
        entity.OnTakeDamage += Entity_OnTakeDamage;
        entity.OnDied += Entity_OnDied;
        entity.OnDestroyed += Entity_OnDestroyed;

        SetType(myType);
    }
    private void FixedUpdate()
    {
        if (myType == Type.Static)
            return;

        timer += Time.deltaTime;

        if (timer >= periodicInterval)
        {
            if (active)
            {
                Disable();
            }
            else
            {
                Enable();
            }
            Debug.Log(active);

            timer -= periodicInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();
        Entity otherEntity = entityManager.GetByInstanceId(other.transform.GetInstanceID());
        if (otherEntity == null)
            return;
        entity.DealDamage(otherEntity, 50.0f);
    }

    private void Entity_OnTakeDamage(Entity me, Entity attacker, float damage)
    {
        // Spikes dont take dmg u dummy
    }
    private void Entity_OnDied(Entity me, Entity killer)
    {
        // Game Over
    }
    private void Entity_OnDestroyed()
    {
        // Cleanup
        entity = null;
    }
    public float periodicInterval;

    private Type myType;
    private float timer;
    private bool active;
    private Entity entity = null;
}
