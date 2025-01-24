namespace GGJ.Player
{
    using UnityEngine;

    public class Controller : MonoBehaviour
    {
        [Header("SETTINGS")]

        [Space]
        [Header("Movement")]
        [SerializeField] private float _movementSpeed;

        [Space]
        [Header("Jumping")]
        [Range(0f, 0.5f)]
        [SerializeField] private float _jumpBuffer = 0.2f;
        [Range(0f, 0.5f)]
        [SerializeField] private float _coyoteTime = 0.2f;
        [SerializeField] private float _jumpForce;

        private Rigidbody2D _rigidbody;
        private LayerMask _groundLayers;

        private int _requestedMovement = 0;
        private float _timeSinceGrounded = 0;
        private float _timeSinceJumpRequest = 0;


        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _groundLayers = LayerMask.NameToLayer("Ground");
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;

            TickTimers(deltaTime);
            UpdateInput();
        }

        private void FixedUpdate()
        {
            HorizontalMovement();
            VerticalMovement();
        }

        private void UpdateInput()
        {
            _requestedMovement = (int)Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space))
                _timeSinceJumpRequest = _jumpBuffer;
        }

        private void TickTimers(float deltaTime)
        {
            _timeSinceGrounded -= deltaTime;
            _timeSinceJumpRequest -= deltaTime;
        }

        private void HorizontalMovement()
        {

        }

        private void VerticalMovement()
        {
            if (_timeSinceGrounded > 0 && _timeSinceJumpRequest > 0)
                PerformJump();
        }

        private void PerformJump()
        {
            _timeSinceGrounded = 0;
            _timeSinceJumpRequest = 0;

            
        }
    }
}