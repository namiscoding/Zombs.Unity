using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    [SerializeField]
    private float _rotationSpeed = 1000f;

    [SerializeField]
    private float _chaseDistanceThreshold = 1f;

    private Rigidbody2D _rigidbody;
    private PlayerAwarenessController _playerAwarenessController;
    private Vector2 _targetDirection;
    private float _changeDirectionCooldown = 1.5f;

    // Thêm biến để theo dõi base
    private Transform _baseTransform;
    private EnemyState _currentState = EnemyState.AttackingBase;
    private EnemyWithWeapon _enemyWithWeapon;
    private EnemyNoWeapon _enemyNoWeapon;

    public Transform armTransform;
    private Vector3 armLocalPosition;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
        _targetDirection = transform.up;
        
        // Tìm base trong scene
        Center center = FindObjectOfType<Center>();
        if (center != null)
        {
            _baseTransform = center.transform;
            _currentState = EnemyState.AttackingBase;
            Debug.Log($"Enemy found base at {_baseTransform.position}");
        }
        else
        {
            Debug.LogWarning("Base not found in scene!");
            _currentState = EnemyState.Idle;
        }

        // Lấy tham chiếu đến component enemy
        _enemyWithWeapon = GetComponent<EnemyWithWeapon>();
        _enemyNoWeapon = GetComponent<EnemyNoWeapon>();
    }

    private void Update()
    {
        // Đồng bộ trạng thái với component enemy
        if (_enemyWithWeapon != null)
        {
            _currentState = _enemyWithWeapon.currentState;
        }
        // else if (_enemyNoWeapon != null)
        // {
        //     _currentState = _enemyNoWeapon.currentState;
        // }

        UpdateTargetDirection();
        // RotateTowardsTarget();
        SetVelocity();
        
        if (armTransform != null)
            armLocalPosition = armTransform.localPosition;
    }

    private void UpdateTargetDirection()
    {
        // Nếu đang tấn công base, hướng về base
        if (_currentState == EnemyState.AttackingBase && _baseTransform != null)
        {
            Vector2 directionToBase = (_baseTransform.position - transform.position).normalized;
            _targetDirection = directionToBase;
            Debug.DrawRay(transform.position, directionToBase * 2f, Color.red); // Vẽ ray để debug
        }
        // Nếu đang tấn công player, sử dụng PlayerAwarenessController
        else if (_currentState == EnemyState.AttackingPlayer && _playerAwarenessController.AwareOfPlayer)
        {
            _targetDirection = _playerAwarenessController.DirectionToPlayer;
        }
        // Nếu không có mục tiêu cụ thể, di chuyển ngẫu nhiên
        else
        {
            HandleRandomDirectionChange();
        }
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

    // private void RotateTowardsTarget()
    // {
    //     float targetAngle = Mathf.Atan2(_targetDirection.y, _targetDirection.x) * Mathf.Rad2Deg - 90f;
    //     Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
    //     transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    // }

    private void SetVelocity()
    {
        // Kiểm tra khoảng cách đến mục tiêu
        float distanceToTarget = 100f; // Giá trị mặc định lớn
        
        if (_currentState == EnemyState.AttackingBase && _baseTransform != null)
        {
            distanceToTarget = Vector2.Distance(transform.position, _baseTransform.position);
        }
        else if (_currentState == EnemyState.AttackingPlayer && _playerAwarenessController.AwareOfPlayer)
        {
            distanceToTarget = Vector2.Distance(transform.position, _playerAwarenessController.PlayerPosition);
        }

        // Nếu đã đến đủ gần mục tiêu, dừng lại
        if (distanceToTarget <= _chaseDistanceThreshold)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        // Di chuyển theo hướng mục tiêu
        _rigidbody.linearVelocity = _targetDirection * _speed;
    }
}