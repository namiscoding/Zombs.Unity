using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed=3f;

    [SerializeField]
    private float _rotationSpeed= 1000f;

    [SerializeField]
    private float _chaseDistanceThreshold = 1f; // Ngưỡng khoảng cách để dừng lại khi đuổi theo người chơi

    private Rigidbody2D _rigidbody;
    private PlayerAwarenessController _playerAwarenessController;
    private Vector2 _targetDirection;
    private float _changeDirectionCooldown = 1.5f;

    public Transform armTransform;
    private Vector3 armLocalPosition;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
        _targetDirection = transform.up;
    }

    private void Update()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
        if (armTransform != null)
            armLocalPosition = armTransform.localPosition;
    }

    private void UpdateTargetDirection()
    {
        HandleRandomDirectionChange();
        HandlePlayerTargeting();
    }

    private void HandleRandomDirectionChange()
    {
        _changeDirectionCooldown -= Time.deltaTime;

        if (_changeDirectionCooldown <= 0)
        {
            float angleChange = Random.Range(-90f, 90f);
            Quaternion rotation = Quaternion.AngleAxis(angleChange, transform.forward);
            _targetDirection = rotation * _targetDirection;

            _changeDirectionCooldown = Random.Range(1f, 5f);
        }
    }

    private void HandlePlayerTargeting()
    {
        if (_playerAwarenessController.AwareOfPlayer)
        {
            // Đổi hướng di chuyển về phía người chơi
            _targetDirection = _playerAwarenessController.DirectionToPlayer;

            // Kiểm tra khoảng cách tới người chơi và dừng lại nếu gần đủ
            float distanceToPlayer = Vector2.Distance(transform.position, _playerAwarenessController.PlayerPosition);
            if (distanceToPlayer <= _chaseDistanceThreshold)
            {
                // Dừng lại khi đã đến gần người chơi
                _rigidbody.linearVelocity = Vector2.zero;
            }
            else
            {
                // Tiếp tục đuổi theo nếu người chơi ra xa
                _rigidbody.linearVelocity = _targetDirection * _speed;
            }
        }
    }

    private void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        _rigidbody.SetRotation(rotation);
    }

    private void SetVelocity()
    {
        // Nếu kẻ thù không dừng lại, tính vận tốc
        if (_rigidbody.linearVelocity.magnitude > 0)
        {
            _rigidbody.linearVelocity = _targetDirection * _speed;
        }
    }
}