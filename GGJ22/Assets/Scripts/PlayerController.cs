using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private enum AnimState
    {
        Idle,
        Run,
        AerialUp,
        AerialDown,
        Die
    }

    private void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _animator = spriteAnimator.GetComponent<Animator>();

        _spriteAnimatorObjectScaleLeft = spriteAnimator.transform.localScale;
        _spriteAnimatorObjectScaleRight = _spriteAnimatorObjectScaleLeft;
        _spriteAnimatorObjectScaleRight.x *= -1;

        if (!_animator)
            Debug.LogWarning("Player is missing its animator!");
    }

    private void Update()
    {
        _inputDirection = Input.GetAxisRaw("Horizontal");

        if (_inputDirection < 0)
        {
            spriteAnimator.transform.localScale = _spriteAnimatorObjectScaleLeft;
        }
        else if (_inputDirection > 0)
        {
            spriteAnimator.transform.localScale = _spriteAnimatorObjectScaleRight;
        }

        // get buttonUP jump

        _jump |= Input.GetButtonDown("Jump");
        _jump &= _canJump;
        _hasReleasedJumpButton |= Input.GetButtonUp("Jump");

        _currGracePeriod = Mathf.Max(_currGracePeriod - Time.deltaTime, 0.0f);
    }

    private void FixedUpdate()
    {
        _canJump = Physics2D.OverlapBox(_jumpCheck.position, _jumpCheckSize, 0.0f, _groundLayerMask) ||
                   _currGracePeriod > 0.0f;

        bool jumpedThisFrame = _canJump && _jump;
        bool isGroundedLastFrame = _isGrounded;

        _isGrounded = Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0.0f, _groundLayerMask);

        if (jumpedThisFrame)
        {
            _jump = false;
            _currGracePeriod = 0.0f;
            _velocity.y = _jumpPower;
            _hasReleasedJumpButton = false;
        }

        if (_isGrounded)
        {
            _velocity.x = _inputDirection * _groundSpeed;
            _velocity.y = Mathf.Max(_velocity.y, 0.0f); // No y velocity going into the ground

            if (_inputDirection != 0)
                _animator.SetInteger("AnimState", (int)AnimState.Run);
            else
                _animator.SetInteger("AnimState", (int)AnimState.Idle);
        }
        else
        {
            _animator.SetInteger("AnimState", (int)AnimState.AerialUp);

            // If player walked off an edge, give them some extra time to jump
            if (!_jumpedLastFrame && isGroundedLastFrame)
            {
                _currGracePeriod = _jumpGracePeriod;
            }


            if (!_hasReleasedJumpButton)
            {
                _velocity.y += _aerialExtraJumpPower;
            }

            // Apply gravity
            _velocity.y = Mathf.Max(_velocity.y - _gravity, -_maxFallVelocity);

            if (_velocity.y > 0)
                _animator.SetInteger("AnimState", (int)AnimState.AerialUp);
            else
                _animator.SetInteger("AnimState", (int)AnimState.AerialDown);

            _velocity.x += _inputDirection * _aerialHorizontalAcceleration;
            _velocity.x = Mathf.Clamp(_velocity.x, -_groundSpeed, _groundSpeed);
        }

        _jumpedLastFrame = jumpedThisFrame;
        Move();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_jumpCheck.position, _jumpCheckSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
    }

    private void Move()
    {
        Vector2 movePosition = _rigidbody.position;
        movePosition += _velocity * Time.fixedDeltaTime;

        _rigidbody.MovePosition(movePosition);
    }

    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private float _inputDirection;
    private Vector2 _velocity;

    private Vector3 _spriteAnimatorObjectScaleLeft;
    private Vector3 _spriteAnimatorObjectScaleRight;

    private bool _jump;
    private bool _canJump;
    private bool _jumpedLastFrame;
    private bool _hasReleasedJumpButton;
    private bool _isGrounded;
    private float _currGracePeriod;

    [SerializeField] private GameObject spriteAnimator;
    [SerializeField] private float _jumpPower = 1.0f;
    [SerializeField] private float _groundSpeed = 1.0f;
    [SerializeField] private float _aerialHorizontalAcceleration = 0.02f;
    [SerializeField] private float _aerialExtraJumpPower = 0.02f;
    //[SerializeField] private float _aerialHorizontalMaxVelocity = 6.0f; // Add this to the clamp check in FixedUpdate->!isGrounded
    [SerializeField] private float _gravity = 0.6f;
    [SerializeField] private float _maxFallVelocity = 20.0f;
    [SerializeField] private float _jumpGracePeriod = 0.1f;

    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private Transform _jumpCheck;
    [SerializeField] private Vector2 _jumpCheckSize;
    [SerializeField] private LayerMask _groundLayerMask;
}