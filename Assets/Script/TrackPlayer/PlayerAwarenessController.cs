    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerAwarenessController : MonoBehaviour
    {
        public bool AwareOfPlayer { get; private set; }
        public Vector2 DirectionToPlayer { get; private set; }
        public Vector2 PlayerPosition { get; private set; } // Thêm thuộc tính để lấy vị trí Player

        [SerializeField]
        private float _playerAwarenessDistance;

        private Transform _player;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        void Update()
        {
            Vector2 enemyToPlayerVector = _player.position - transform.position;
            DirectionToPlayer = enemyToPlayerVector.normalized;
            PlayerPosition = _player.position; // Cập nhật vị trí Player

            if (enemyToPlayerVector.magnitude <= _playerAwarenessDistance)
            {
                AwareOfPlayer = true;
            }
            else
            {
                AwareOfPlayer = false;
            }
        }
    }