using UnityEngine;

public struct SpiritOrbEvent
{
    public float secondsGained;
}
public class SpiritOrb : MonoBehaviour
{
    private void Start()
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        _entity = entityManager.Create(GetInstanceID());

        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("Spirit Orb is missing a Sprite Renderer");
        }
    }
    private void Update()
    {
        if (!_consumed)
            return;

        _fadeOutTimer = Mathf.Min(_fadeOutTimer + Time.deltaTime, _fadeOutMaxTime);

        if (IsFullyConsumed())
        {
            FinalizeConsumeState();
        }
        else
        {
            UpdateConsumeState();
        }
    }

    private bool IsConsumsed()
    {
        return _consumed;
    }
    private void StartConsume()
    {
        _consumed = true;

        EntityManager entityManager = ServiceLocator.GetEntityManager();
        Entity playerEntity = entityManager.GetByReference(EntityReference.LocalplayerBody);

        Animator animator = GetComponent<Animator>();
        animator.Play("SpiritOrbConsumed");

        SpiritOrbEvent spiritOrbEvent = new SpiritOrbEvent
        {
            secondsGained = _spiritEnergiSecondGained
        };
        _entity.Interact(playerEntity, spiritOrbEvent);
    }
    private bool IsFullyConsumed()
    {
        return _fadeOutTimer == _fadeOutMaxTime;
    }
    private void UpdateConsumeState()
    {
        float progress = _fadeOutTimer / _fadeOutMaxTime;
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, Mathf.Lerp(1.0f, 0.0f, progress));
    }
    private void FinalizeConsumeState()
    {
        _consumed = false;
        gameObject.SetActive(false);

        _fadeOutTimer = 0.0f;
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsConsumsed() || !collision.CompareTag("Player"))
            return;

        StartConsume();
    }

    [SerializeField] private bool _consumed = false;
    [SerializeField] private float _fadeOutMaxTime = 0.85f;
    [SerializeField] private float _fadeOutTimer = 0f;
    [SerializeField] private float _spiritEnergiSecondGained = 2.5f;

    private SpriteRenderer _spriteRenderer;
    private Entity _entity = null;
}
