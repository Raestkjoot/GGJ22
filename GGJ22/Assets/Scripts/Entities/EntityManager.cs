using System.Collections.Generic;

public delegate void EntityTakeDamageEventHandler(Entity me, Entity attacker, float damage);
public delegate void EntityDiedEventHandler(Entity me, Entity killer);
public delegate void EntityDestroyedEventHandler();

public class Entity
{
    public int id;

    public float currentHealth = 100;
    public float maxHealth     = 100;

    public void DealDamage(Entity victim, float damage)
    {
        if (currentHealth <= 0)
            return;

        if (victim.OnTakeDamage != null)
        {
            victim.OnTakeDamage(victim, this, damage);

            if (victim.currentHealth <= 0)
            {
                if (victim.OnDied != null)
                    victim.OnDied(victim, this);
            }
        }
    }
    public event EntityTakeDamageEventHandler OnTakeDamage;

    public void Died(Entity killer)
    {
        if (OnDied != null)
            OnDied(this, killer);
    }
    public event EntityDiedEventHandler OnDied;

    public void Destroy()
    {
        if (OnDestroyed != null)
            OnDestroyed();
    }
    public event EntityDestroyedEventHandler OnDestroyed;
}

public class EntityManager
{
    public void Initialize(Bootstrap bootstrap)
    {
        // Preallocate memory for 128 entities.

        _entities = new List<Entity>(128);
        _instanceIdToEntityMap = new Dictionary<int, Entity>(128);
    }

    public Entity Create(int instanceID)
    {
        Entity entity = new Entity
        {
            id = _entities.Count
        };

        _entities.Add(entity);
        _instanceIdToEntityMap[instanceID] = entity;
        return entity;
    }
    public void Destroy(int id)
    {
        Entity entity = _entities[id];
        Destroy(entity);
    }
    public void Destroy(Entity entity)
    {
        entity.Destroy();

        _entities.RemoveAt(entity.id);
        _instanceIdToEntityMap.Remove(entity.id);
    }

    public Entity GetById(int id)
    {
        return _entities[id];
    }
    public Entity GetByInstanceId(int instanceId)
    {
        return _instanceIdToEntityMap[instanceId];
    }

    public void Clear()
    {
        _entities.Clear();
        _instanceIdToEntityMap.Clear();
    }

    private List<Entity> _entities;
    private Dictionary<int, Entity> _instanceIdToEntityMap;
}