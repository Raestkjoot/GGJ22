using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
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
            _verticalVelocity = _jumpPower;
        }

        if (_isGrounded)
        {
            _horizontalVelocity = _inputDirection * _groundSpeed;
            _verticalVelocity = Mathf.Max(_verticalVelocity, 0.0f); // No negative vertical velocity
        }
        else
        {
            if (!_jumpedLastFrame && isGroundedLastFrame)
            {
                _currGracePeriod = _jumpGracePeriod;
            }

            _verticalVelocity = Mathf.Max(_verticalVelocity - _gravity * Time.fixedDeltaTime, -_maxFallVelocity);

            _horizontalVelocity += _inputDirection * _aerialHorizontalAcceleration;
            _horizontalVelocity = Mathf.Clamp(_horizontalVelocity, -_groundSpeed, _groundSpeed);
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
        movePosition.x += _horizontalVelocity * Time.fixedDeltaTime;
        movePosition.y += _verticalVelocity * Time.fixedDeltaTime;
        _rigidbody.MovePosition(movePosition);
    }

    private Rigidbody2D _rigidbody;

    private float _inputDirection;
    // TODO vector2
    private float _horizontalVelocity;
    private float _verticalVelocity;

    private bool _jump;
    private bool _canJump;
    private bool _jumpedLastFrame;
    private bool _isGrounded;
    private float _currGracePeriod;

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