using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private void Start()
    {
        EntityManager entityManager = ServiceLocator.GetEntityManager();

        _entity = entityManager.Create(transform.GetInstanceID());
        entityManager.CreateReference(_entity, EntityReference.LocalplayerBody);

        _entity.OnTakeDamage += Entity_OnTakeDamage;
        _entity.OnDied += Entity_OnDied;
        _entity.OnInteract += Entity_OnInteract;
        _entity.OnDestroyed += Entity_OnDestroyed;

        if (_heartImage == null)
        {
            Debug.LogError("Player is missing Heart Image (A reference to the sprite which displays the health)");
        }

        if (_heartSprites == null || _heartSprites.Length != _maxHeartSprites)
        {
            Debug.LogError("Player is missing Heart Image (A reference to the sprite which displays the health)");
        }

        if (_spiritSlider == null)
        {
            Debug.LogError("Player is missing Spirit Slider (A reference to the Parent Gamobject 'SpiritMeterFrame')");
        }

        if (_spiritSliderRect == null)
        {
            Debug.LogError("Player is missing Spirit Slider (A reference to the Rect Transform attached to SpiritMeterFill)");
        }

        UpdateSpiritSliderState(false, 0.0f, 0.0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ToggleSpiritState();
        }

        if (IsInSpiritForm())
        {
            _spiritSliderTimer = Mathf.Min(_spiritSliderTimer + Time.deltaTime, _spiritSliderMaxTime);
            if (_spiritSliderTimer == _spiritSliderMaxTime)
            {
                // TODO : Implement what happens when you run out of spirit energy
                ToggleSpiritState();
            }
        }
    }

    private void OnGUI()
    {
        UpdateSpiritSliderState(IsInSpiritForm(), _spiritSliderTimer, GetSliderProgress());
    }

    private void Entity_OnTakeDamage(Entity me, Entity attacker, float damage)
    {
        if (IsInSpiritForm())
            return;

        // Reduce health by damage, but never below 0
        me.currentHealth = Mathf.Max(me.currentHealth - damage, 0);

        int numHearts = GetHeartsFromHealth(me.currentHealth);
        _heartImage.sprite = _heartSprites[numHearts];
    }
    private void Entity_OnDied(Entity me, Entity killer)
    {
        int numHearts = 0;
        _heartImage.sprite = _heartSprites[numHearts];

        ServiceLocator.GetGameHandler().ResetMap();
    }
    private void Entity_OnInteract(Entity me, Entity other, object arg)
    {
        if (arg is SpiritOrbEvent)
        {
            SpiritOrbEvent spiritOrbEvent = (SpiritOrbEvent)arg;
            _spiritSliderTimer = Mathf.Max(_spiritSliderTimer - spiritOrbEvent.secondsGained, 0.0f);
            UpdateSpiritSliderState(IsInSpiritForm(), _spiritSliderTimer, GetSliderProgress());
        }
    }
    private void Entity_OnDestroyed()
    {
        // Cleanup
        EntityManager entityManager = ServiceLocator.GetEntityManager();
        entityManager.DestroyReference(EntityReference.LocalplayerBody);

        _entity = null;
    }

    private int GetHeartsFromHealth(float health)
    {
        return (int)((health + 32.33f) / 33.33f);
    }

    private void ToggleSpiritState()
    {

        if (IsInSpiritForm())
        {
            LeaveSpiritForm();
        }
        else
        {
            EnterSpiritForm();
        }

        _isInSpiritForm = !_isInSpiritForm;
        UpdateSpiritSliderState(_isInSpiritForm, 0.0f, 0.0f);
    }

    // coroutines?
    private void EnterSpiritForm()
    {
        // spawn lifeless player body
        _mortalCoilPosition = Instantiate(_theMortalCoil, transform.position, transform.rotation);
        _bodyAnimatorObject.SetActive(false);
        _ghostAnimatorObject.SetActive(true);

        _bodyPlayerController.enabled = false;
        _ghostPlayerController.enabled = true;

        ServiceLocator.GetGameHandler().EnterSpiritRealm();

    }
    private void LeaveSpiritForm()
    {
        Vector3 respawnLocation = _mortalCoilPosition.transform.position;
        respawnLocation.y += 1;
        transform.position = respawnLocation;
        Destroy(_mortalCoilPosition);

        _bodyAnimatorObject.SetActive(true);
        _ghostAnimatorObject.SetActive(false);

        _bodyPlayerController.enabled = true;
        _ghostPlayerController.enabled = false;

        ServiceLocator.GetGameHandler().LeaveSpiritRealm();
    }
    private void UpdateSpiritSliderState(bool state, float timer, float progress)
    {
        _spiritSlider.SetActive(state);

        _spiritSliderTimer = timer;
        float progressLerped = -Mathf.Lerp(0.0f, 500.0f, progress);
        _spiritSliderRect.offsetMax = new Vector2(progressLerped, _spiritSliderRect.offsetMax.y);
    }
    private float GetSliderProgress()
    {
        return _spiritSliderTimer / _spiritSliderMaxTime;
    }
    private bool IsInSpiritForm() { return _isInSpiritForm; }

    private GameObject _mortalCoilPosition;

    private const int _maxHeartSprites = 4;
    [SerializeField] private Sprite[] _heartSprites;
    [SerializeField] private Image _heartImage;

    [SerializeField] private bool _isInSpiritForm = false;
    [SerializeField] private GameObject _spiritSlider;
    [SerializeField] private RectTransform _spiritSliderRect;
    [SerializeField] private float _spiritSliderMaxTime = 5.0f;
    [SerializeField] private float _spiritSliderTimer = 0.0f;

    [SerializeField] private GameObject _bodyAnimatorObject;
    [SerializeField] private PlayerController _bodyPlayerController;
    [SerializeField] private GameObject _ghostAnimatorObject;
    [SerializeField] private PlayerController _ghostPlayerController;
    [SerializeField] private GameObject _theMortalCoil;

    private Entity _entity = null;
}
