using UnityEngine;

public class Player : MonoBehaviour
{
    private void Start()
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        _entity = entityManager.Create(GetInstanceID());
        entityManager.CreateReference(_entity, EntityReference.LocalplayerBody);

        _entity.OnTakeDamage += Entity_OnTakeDamage;
        _entity.OnDied += Entity_OnDied;
        _entity.OnDestroyed += Entity_OnDestroyed;
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
        EntityManager entityManager = ServiceLocator.GetEntityManager();
        entityManager.DestroyReference(EntityReference.LocalplayerBody);

        _entity = null;
    }

    private Entity _entity = null;
}
