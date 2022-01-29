using UnityEngine;

public delegate void BootstrapEventHandler();
public class Bootstrap : MonoBehaviour
{
    public event BootstrapEventHandler OnDestroy;
    public event BootstrapEventHandler OnApplicationQuit;

    void Start()
    {
        GameHandler gameHandler = new GameHandler();
        gameHandler.Initialize(this);
        ServiceLocator.SetGameHandler(gameHandler);
        
        EntityManager entityManager = new EntityManager();
        entityManager.Initialize(this);
        ServiceLocator.SetEntityManager(entityManager);

        entityManager.Create(GetInstanceID());
        entityManager.Create(1);

        Entity entity1 = entityManager.GetByInstanceId(GetInstanceID());
        Entity entity2 = entityManager.GetByInstanceId(1);

        entity1.OnTakeDamage += Entity1_OnTakeDamage;
        entity1.OnDied += Entity1_OnDied;

        entity2.OnTakeDamage += Entity2_OnTakeDamage;
        entity2.OnDied += Entity2_OnDied;

        entity1.DealDamage(entity2, 101);
        entity2.DealDamage(entity1, 50.5f);
    }

    private void Entity1_OnTakeDamage(Entity me, Entity attacker, float damage)
    {
        float trueDamage = damage * 2.0f;
        me.currentHealth = Mathf.Max(me.currentHealth - trueDamage, 0);

        Debug.Log($"Entity 1 took {trueDamage} damage from attacker!!!");
    }

    private void Entity1_OnDied(Entity me, Entity killer)
    {
        Debug.Log($"Entity 1 died!!!");
    }

    private void Entity2_OnTakeDamage(Entity me, Entity attacker, float damage)
    {
        me.currentHealth = Mathf.Max(me.currentHealth - damage, 0);

        Debug.Log($"Entity 2 took {damage} damage from attacker!!!");
    }

    private void Entity2_OnDied(Entity me, Entity killer)
    {
        Debug.Log($"Entity 2 died!!!");
    }
}
