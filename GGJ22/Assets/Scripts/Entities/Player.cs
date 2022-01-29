using UnityEngine;

public class Player : MonoBehaviour
{
    private void Start()
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        entity = entityManager.Create(GetInstanceID());
        entity.OnTakeDamage += Entity_OnTakeDamage;
        entity.OnDied += Entity_OnDied;
        entity.OnDestroyed += Entity_OnDestroyed;
    }

    private void Entity_OnTakeDamage(Entity me, Entity attacker, float damage)
    {
        // Reduce health by damage, but never below 0
        me.currentHealth = Mathf.Max(me.currentHealth - damage, 0);
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

    private Entity entity = null;
}
