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
        _active = true;
    }
    public void Disable() 
    {
        _active = false;
    }

    public void SetType(Type spikeType) 
    {
        _spikeType = spikeType;
        _timer = 0.0f;

        if (_spikeType == Type.Static)
            Enable();
    }
    public Type GetSpikeType() { return _spikeType; }

    public void SetPeriodic(float interval) 
    {
        float oldInterval = periodicInterval;
        periodicInterval = interval;
        _timer = MathUtils.Map(_timer, 0.0f, oldInterval, 0.0f, interval);
    }

    private void Start()
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        _entity = entityManager.Create(transform.GetInstanceID());
        _entity.OnTakeDamage += Entity_OnTakeDamage;
        _entity.OnDied += Entity_OnDied;
        _entity.OnDestroyed += Entity_OnDestroyed;

        SetType(_spikeType);
    }
    private void FixedUpdate()
    {
        if (_spikeType == Type.Static)
            return;

        _timer += Time.deltaTime;

        if (_timer >= periodicInterval)
        {
            if (_active)
            {
                Disable();
            }
            else
            {
                Enable();
            }

            _timer -= periodicInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        Entity otherEntity = entityManager.GetByInstanceId(other.transform.GetInstanceID());
        if (otherEntity == null)
            return;

        _entity.DealDamage(otherEntity, 50.0f);
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
        _entity = null;
    }

    public float periodicInterval;

    [SerializeField]
    private Type _spikeType;
    private float _timer;
    private bool _active;

    private Entity _entity = null;
}
