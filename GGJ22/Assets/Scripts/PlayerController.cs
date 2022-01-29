using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private enum animState
    {
        Idle,
        RunLeft,
        RunRight,
        Jump,
        Die
    }

    private void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();

        if (_animator)
            _animator.SetInteger("AnimState", (int)animState.Idle);
        else
            Debug.LogWarning("Player is missing its animator!");
    }

    private void Update()
    {
        _inputDirection = Input.GetAxisRaw("Horizontal");

        _jump |= Input.GetButtonDown("Jump");
        _jump &= _canJump;

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
        }

        if (_isGrounded)
        {
            _velocity.x = _inputDirection * _groundSpeed;
            _velocity.y = Mathf.Max(_velocity.y, 0.0f); // No y velocity going into the ground

            if (_inputDirection > 0)
                _animator.SetInteger("AnimState", (int)animState.RunRight);
            else if (_inputDirection < 0)
                _animator.SetInteger("AnimState", (int)animState.RunLeft);
            else // _inputDirection == 0
                _animator.SetInteger("AnimState", (int)animState.Idle);
        }
        else
        {
            _animator.SetInteger("AnimState", (int)animState.Jump);

            // If player walked off an edge, give them some extra time to jump
            if (!_jumpedLastFrame && isGroundedLastFrame)
            {
                _currGracePeriod = _jumpGracePeriod;
            }

            // Apply gravity
            _velocity.y = Mathf.Max(_velocity.y - _gravity * Time.fixedDeltaTime, -_maxFallVelocity);
            
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

    private float _inputDirection;
    private Vector2 _velocity;

    private bool _jump;
    private bool _canJump;
    private bool _jumpedLastFrame;
    private bool _isGrounded;
    private float _currGracePeriod;

    [SerializeField] private Animator _animator;
    [SerializeField] private float _jumpPower = 1.0f;
    [SerializeField] private float _groundSpeed = 1.0f;
    [SerializeField] private float _aerialHorizontalAcceleration = 0.02f;
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