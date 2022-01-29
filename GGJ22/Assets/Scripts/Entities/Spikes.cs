using UnityEngine;

public class Spikes : MonoBehaviour
{
    public enum Type
    {
        Disabled,
        Static,
        Periodic,
        PeriodicSuppressed
    }
    public enum InteractMethod
    {
        Suppressed,
        Disabled
    }

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
        _entity.OnInteract += Entity_OnInteract;
        _entity.OnDestroyed += Entity_OnDestroyed;

        if (_defaultType == Type.Disabled)
        {
            _defaultType = Type.Static;
            Debug.LogError($"Game object ({gameObject.name}) has invalid spike type 'Disabled', defaulting to 'Static'.");
        }

        if (_disableOnStart)
        {
            if (_defaultType == Type.Static)
            {
                Disable();
            }
            else if (_defaultType == Type.Periodic)
            {
                Suppress();
            }
        }
        else
        { 
            _currentType = _defaultType;
        }
    }
    private void FixedUpdate()
    {
        if (_defaultType != Type.Periodic)
            return;

        _timer += Time.deltaTime;

        if (_timer >= periodicInterval)
        {
            _timer -= periodicInterval;

            if (_currentType == Type.Periodic)
            {
                Toggle();
            }
            else if (_currentType == Type.PeriodicSuppressed)
            {
                _currentType = Type.Periodic;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsSpikeEnabled())
            return;

        EntityManager entityManager = ServiceLocator.GetEntityManager();

        Entity otherEntity = entityManager.GetByInstanceId(other.transform.GetInstanceID());
        if (otherEntity == null)
            return;

        _entity.DealDamage(otherEntity, 50.0f);
    }

    private void Entity_OnInteract(Entity me, Entity other, object arg)
    {
        // When being interacted with, we want the entity to enable/disable
        if (_defaultType == Type.Static)
        {
            Toggle();
        }
        else if (_defaultType == Type.Periodic)
        {
            if (_currentType == Type.Disabled || _currentType == Type.PeriodicSuppressed)
                return;
            
            InteractMethod interactMethod = (InteractMethod)arg;
            if (interactMethod == InteractMethod.Suppressed)
            {
                Suppress();
            }
            else if (interactMethod == InteractMethod.Disabled)
            {
                // TODO:
                // Play inactive animation 
                _currentType = Type.Disabled;
            }
        }
    }
    private void Entity_OnDestroyed()
    {
        // Cleanup
        _entity = null;
    }

    private void Toggle()
    {
        if (_defaultType == Type.Periodic)
            return;

        if (_currentType == Type.Static)
        {
            Disable();
        }
        else
        {
            Enable();
        }
    }
    private void Enable()
    {
        _currentType = _defaultType;
    }
    private void Disable() 
    {
        _currentType = Type.Disabled;
    }
    private void Suppress()
    {
        if (_defaultType != Type.Periodic)
            return;
        
        _currentType = Type.PeriodicSuppressed;
        _timer = 0.0f;
    }
    private bool IsSpikeEnabled() 
    {
        return _currentType != Type.Disabled && _currentType != Type.PeriodicSuppressed;
    }

    public float periodicInterval;

    [SerializeField]
    private Type _defaultType = Type.Static;
    [SerializeField]
    private Type _currentType;
    private float _timer = 0.0f;
    [SerializeField]
    private bool _disableOnStart = false;
    private Entity _entity = null;
}
