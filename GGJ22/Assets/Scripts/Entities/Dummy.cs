using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        entity = entityManager.Create(transform.GetInstanceID());
        entity.OnTakeDamage += Entity_OnTakeDamage;
        entity.OnDied += Entity_OnDied;
        entity.OnDestroyed += Entity_OnDestroyed;

        startPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
        if (distance <= Vector3.Distance(startPos, transform.position))
            transform.position = startPos;
    }

    private void Entity_OnTakeDamage(Entity me, Entity attacker, float damage)
    {
        me.currentHealth = Mathf.Max(me.currentHealth - damage, 0.0f);
    }
    private void Entity_OnDied(Entity me, Entity killer)
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();
        entityManager.Destroy(me);

        Destroy(gameObject);
    }
    private void Entity_OnDestroyed()
    {
        entity = null;
    }

    public float distance;
    public float speed;
    private Vector3 startPos;
    Entity entity;
}
