using System.Collections.Generic;

public delegate void EntityTakeDamageEventHandler(Entity me, Entity attacker, float damage);
public delegate void EntityDiedEventHandler(Entity me, Entity killer);
public delegate void EntityDestroyedEventHandler();

public class Entity
{
    public int id;
    public int instanceId;

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

public enum EntityReference
{
    LocalplayerBody,
    LocalplayerSoul
}
public class EntityManager
{
    public void Initialize(Bootstrap bootstrap)
    {
        // Preallocate memory for 128 entities.

        _entities = new List<Entity>(128);
        _instanceIdToEntityMap = new Dictionary<int, Entity>(128);
        _referenceToEntityMap = new Dictionary<EntityReference, Entity>(8);
    }

    public Entity Create(int entityInstanceId)
    {
        Entity entity = new Entity
        {
            id = _entities.Count,
            instanceId = entityInstanceId
        };

        _entities.Add(entity);
        _instanceIdToEntityMap[entityInstanceId] = entity;
        return entity;
    }
    public bool CreateReference(Entity entity, EntityReference reference)
    {
        if (_referenceToEntityMap.ContainsKey(reference))
            return false;

        _referenceToEntityMap[reference] = entity;
        return true;
    }
    public void Destroy(Entity entity)
    {
        entity.Destroy();

        _entities.RemoveAt(entity.id);
        _instanceIdToEntityMap.Remove(entity.instanceId);
    }
    public bool DestroyReference(EntityReference reference)
    {
        return _referenceToEntityMap.Remove(reference);
    }

    public Entity GetById(int id)
    {
        Entity entity = null;
        if (id < _entities.Count)
            entity = _entities[id];

        return entity;
    }
    public Entity GetByInstanceId(int instanceId)
    {
        Entity entity = null;
        _instanceIdToEntityMap.TryGetValue(instanceId, out entity);

        return entity;
    }
    public Entity GetByReference(EntityReference reference)
    {
        Entity entity = null;
        if (_referenceToEntityMap.ContainsKey(reference))
            entity = _referenceToEntityMap[reference];

        return entity;
    }

    public void Clear()
    {
        _entities.Clear();
        _instanceIdToEntityMap.Clear();
    }

    private List<Entity> _entities;
    private Dictionary<int, Entity> _instanceIdToEntityMap;
    private Dictionary<EntityReference, Entity> _referenceToEntityMap;
}