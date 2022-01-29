using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        _entity = entityManager.Create(transform.GetInstanceID());
        _entity.OnTakeDamage += Entity_OnTakeDamage;
        _entity.OnDied += Entity_OnDied;
        _entity.OnDestroyed += Entity_OnDestroyed;

        _startPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += speed * Time.deltaTime * Vector3.down;

        if (distance <= Vector3.Distance(_startPos, transform.position))
            transform.position = _startPos;
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
        _entity = null;
    }

    public float distance;
    public float speed;

    private Vector3 _startPos;
    private Entity _entity;
}
