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

        _animator = gameObject.GetComponent<Animator>();
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
    private void Update()
    {
        if (_defaultType != Type.Periodic)
            return;

        _timer += Time.deltaTime;

        if (_currentType == Type.PeriodicSuppressed)
        {
            if (_timer >= suppressInterval)
            {
                _timer -= suppressInterval;

                Enable();
            }
        }
        else
        {
            if (_timer >= periodicInterval)
            {
                _timer -= periodicInterval;

                Toggle();
            }
        } 
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player")
            return;
            
        if (!IsSpikeEnabled())
            return;

        EntityManager entityManager = ServiceLocator.GetEntityManager();

        Entity otherEntity = entityManager.GetByInstanceId(other.transform.GetInstanceID());
        if (otherEntity == null)
            return;

        _entity.DealDamage(otherEntity, 33.33f);
    }
    private void Entity_OnInteract(Entity me, Entity other, object arg)
    {
        // When being interacted with, we want the entity to enable/disable
        if (_defaultType == Type.Static)
        {
            InteractMethod interactMethod = (InteractMethod)arg;
            if (interactMethod == InteractMethod.Suppressed)
            {
                Toggle();
            }
            else if (interactMethod == InteractMethod.Disabled)
            {
                Disable();
            }
        }
        else if (_defaultType == Type.Periodic)
        {
            if (_currentType == Type.PeriodicSuppressed)
            {
                _timer = 0.0f;
                return;
            }
            InteractMethod interactMethod = (InteractMethod)arg;
            if (interactMethod == InteractMethod.Suppressed)
            {
                _timer = 0.0f;
                Suppress();
            }
            else if (interactMethod == InteractMethod.Disabled)
            {
                Disable();
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
        if (_currentType == Type.Static || _currentType == Type.Periodic)
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
        foreach(Collider2D c in GetComponents<Collider2D> ()) {
            c.enabled = true;
        }
        _animator.SetBool("Up", true);
        _currentType = _defaultType;
    }
    private void Disable() 
    {
        foreach(Collider2D c in GetComponents<Collider2D> ()) {
            c.enabled = false;
        }
        _animator.SetBool("Up", false);
        _currentType = Type.Disabled;
    }
    private void Suppress()
    {
        if (_defaultType != Type.Periodic)
            return;
        
        foreach(Collider2D c in GetComponents<Collider2D> ()) 
        {
            c.enabled = false;
        }

        _animator.SetBool("Up", false);
        _currentType = Type.PeriodicSuppressed;
        _timer = 0.0f;
    }
    private bool IsSpikeEnabled() 
    {
        return _currentType != Type.Disabled && _currentType != Type.PeriodicSuppressed;
    }

    public float periodicInterval;

    [SerializeField] private float suppressInterval;
    private Animator _animator; 
    [SerializeField] private Type _defaultType = Type.Static;
    private Type _currentType;
    [SerializeField] private bool _disableOnStart = false;
    private float _timer = 0.0f;
    private Entity _entity = null;
}
