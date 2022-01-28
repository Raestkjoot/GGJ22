using System.Collections.Generic;
using UnityEngine;

public delegate void EntityTakeDamageEventHandler(Entity attacker, float damage);
public delegate void EntityDiedEventHandler(Entity killer);

public class Entity
{
    public int id;

    public float currentHealth;
    public float maxHealth;

    public void TakeDamage(Entity attacker, float damage)
    {
        if (OnTakeDamage != null)
            OnTakeDamage(attacker, damage);
    }
    public event EntityTakeDamageEventHandler OnTakeDamage;

    public void Died(Entity killer)
    {
        if (OnDied != null)
            OnDied(killer);
    }
    public event EntityDiedEventHandler OnDied;
}

public class EntityManager
{
    private List<Entity> _entities;

    public void Initialize(Bootstrap bootstrap)
    {
        // Preallocate memory for 128 entities.
        _entities = new List<Entity>(128);

        Debug.Log("EntityManager : Initialized");
    }

    public Entity Create()
    {
        Entity entity = new Entity
        {
            id = _entities.Count
        };

        _entities.Add(entity);
        return entity;
    }

    public void DealDamage(Entity attacker, Entity victim, float damage)
    {
        victim.TakeDamage(attacker, damage);

        if (victim.currentHealth <= 0)
        {
            victim.Died(attacker);
        }
    }
}