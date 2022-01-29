using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private void Update()
    {
        _inputDirection = Input.GetAxisRaw("Horizontal");

        _jump |= Input.GetButtonDown("Jump");
        _jump &= _canJump;
    }

    private void FixedUpdate()
    {
        _canJump = Physics2D.OverlapBox(_jumpCheck.position, _jumpCheckSize, 0f, _groundLayerMask);
        _isGrounded = Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0f, _groundLayerMask);

        if (_canJump && _jump)
        {
            _jump = false;
            _verticalVelocity = _jumpPower;
        }

        if (_isGrounded)
        {
            _verticalVelocity = Mathf.Max(_verticalVelocity, 0);

            Move(_groundSpeed);
        }
        else
        {
            _verticalVelocity -= _gravity;

            Move(_groundSpeed * _airControl);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_jumpCheck.position, _jumpCheckSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
    }

    private void Move(float speed)
    {
        Vector2 movePosition = m_rigidbody.position;
        movePosition.x += _inputDirection * speed * Time.fixedDeltaTime;
        movePosition.y += _verticalVelocity * Time.fixedDeltaTime;
        m_rigidbody.MovePosition(movePosition);
    }

    private Rigidbody2D m_rigidbody => gameObject.GetComponent<Rigidbody2D>();

    private float _inputDirection;
    private float _verticalVelocity;

    private bool _jump;
    private bool _canJump;
    private bool _isGrounded;

    [SerializeField] private float _jumpPower = 1.0f;
    [SerializeField] private float _groundSpeed = 1.0f;
    [SerializeField] [Range(0.0f, 1.0f)] private float _airControl = 0.6f;
    [SerializeField] private float _gravity = 0.6f;

    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private Transform _jumpCheck;
    [SerializeField] private Vector2 _jumpCheckSize;
    [SerializeField] private LayerMask _groundLayerMask;
}
