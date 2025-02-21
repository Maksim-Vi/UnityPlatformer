using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using System.Collections.Generic;
using Utilits;

namespace Platformer
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        public InputReader input => _input;
        public GroundChecker groundChecker => _groundChecker;
        public Animator animatior => _animatior;
        public Rigidbody rb => _rb;
        public float currentSpeed => _currentSpeed;

        [Header("References")]
        [SerializeField, Self] Rigidbody _rb;
        [SerializeField, Self] Animator _animatior;
        [SerializeField, Self] PlayerClimbingSystem _playerClimbingSystem;
        [SerializeField, Self] GroundChecker _groundChecker;
        [SerializeField, Anywhere] CinemachineFreeLook _freeLookVCam;
        [SerializeField, Anywhere] InputReader _input;

        [Header("References Move")]
        [SerializeField] private float _moveSpeed = 200f;
        [SerializeField] private float _speedRotation = 300f;
        [SerializeField] private float _smoothTime = 0.2f;

        [Header("References Jump")]
        [SerializeField] private bool isCanJump = true;
        [SerializeField] private float _jumpForce = 5f;
        [SerializeField] private float _jumpDuration = 0.5f;
        [SerializeField] private float _jumpCooldown = 0f;
        [SerializeField] private float _gravityMultiplayer = 3f;

        [Header("References Attack")]
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float attackDictance = 1f;
        [SerializeField] private int damageAmount = 10;
 
        Transform _mainCamera;
        const float ZeroF = 0f;
        float _currentSpeed = 0f;
        float velocity = 0f;
        float jumpVelocity = 0f;

        Vector3 movement;

        List<Timer> timers;
        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;
        CountdownTimer attackTimer;

        StateMachine _stateMachine;

        private void Awake() 
        {
            _playerClimbingSystem.Init(this);
            _mainCamera = Camera.main.transform;
            _freeLookVCam.Follow = transform;
            _freeLookVCam.LookAt = transform;
            _freeLookVCam.OnTargetObjectWarped(transform, transform.position - _freeLookVCam.transform.position - Vector3.forward);

            _rb.freezeRotation = true;

            SetupTimers();

            //State Machine
            _stateMachine = new StateMachine();

            //initStates
            var _runState = new RunState(this, _animatior);
            var _jumpState = new JumpState(this, _animatior);
            var _attackState = new AttackState(this, _animatior);

            //Define transition
            At(_runState, _jumpState, new FuncPredicateBase(() => jumpTimer.IsRunning));
            At(_runState, _attackState, new FuncPredicateBase(() => attackTimer.IsRunning));
            At(_attackState, _runState, new FuncPredicateBase(() => !attackTimer.IsRunning));
            Any(_runState, new FuncPredicateBase(() =>  _groundChecker.IsGround && !jumpTimer.IsRunning && !attackTimer.IsRunning));

            _stateMachine.SetState(_runState);
        }

        private void Start()
        {
            _input.EnablePlayerActions();
        }

        private void OnEnable() 
        {
            _input.Move += OnMove;
            _input.Jump += OnJump;
            _input.Attack += OnAttack;
        }

        private void OnDisable() 
        {
            _input.Move -= OnMove;
            _input.Jump -= OnJump;
            _input.Attack -= OnAttack;
        }

        private void Update() 
        {
            movement = new Vector3(_input.Direction.x, 0f, _input.Direction.y);
            
            HandleTimers();
            _stateMachine.Update();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        void SetupTimers() {
            // Setup timers
            jumpTimer = new CountdownTimer(_jumpDuration);
            jumpCooldownTimer = new CountdownTimer(_jumpCooldown);

            jumpTimer.OnTimerStart += () => jumpVelocity = _jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();

            attackTimer = new CountdownTimer(_attackCooldown);

            timers = new(5) {jumpTimer, jumpCooldownTimer, attackTimer};
        }

        private void OnMove(Vector2 vector)
        {

        } 

        void OnJump(bool performed) {
            if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && _groundChecker.IsGround) {
                jumpTimer.Start();
            }
        }

        void OnAttack() {
            if (!attackTimer.IsRunning) {
                attackTimer.Start();
            }
        }

        public void HandleIdle()
        {
            if(rb.isKinematic || jumpTimer.IsRunning || jumpCooldownTimer.IsRunning) return;

            SmoothSpeed(ZeroF);
            _rb.velocity = new Vector3(ZeroF, _rb.velocity.y, ZeroF);
        }

        public void HandleMovement()
        {
            //rotate movement to match camera direction
            var adjustedDirection = Quaternion.AngleAxis(_mainCamera.eulerAngles.y, Vector3.up) * movement;

            if(adjustedDirection.magnitude > ZeroF)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(1f);
            } else {
                HandleIdle();
            }
        }

        public void HandleJump()
        {
            if(rb.isKinematic) return;

            if (!jumpTimer.IsRunning && groundChecker.IsGround) {
                jumpVelocity = ZeroF;
                rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
                return;
            }

            if (!jumpTimer.IsRunning) 
            {
                // Gravity takes over
                jumpVelocity += Physics.gravity.y * _gravityMultiplayer * Time.fixedDeltaTime;
            }

            jumpVelocity = Mathf.Max(jumpVelocity, Physics.gravity.y * _gravityMultiplayer);
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }

        private void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            if(rb.isKinematic) return;

            Vector3 velocity = adjustedDirection * _moveSpeed * Time.deltaTime;
            _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z);
        }

        private void HandleRotation(Vector3 adjustedDirection)
        {
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _speedRotation * Time.deltaTime);
            transform.LookAt(transform.position + adjustedDirection);
        }

        public void Attack()
        {
            Vector3 attackPosition = transform.position + transform.forward;
            Collider[] hitEnnimies = Physics.OverlapSphere(attackPosition, attackDictance);

            foreach (var enemy in hitEnnimies)
            {
                if(enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<HealthSystem>().TakeDamage(damageAmount);
                }
            }
        }

        void SmoothSpeed(float val)
        {
            _currentSpeed = val;
        }

        public void AnimationChange()
        {
            _animatior.SetFloat("Speed", _currentSpeed);
        }

        void HandleTimers() {
            foreach (var timer in timers) {
                timer.Tick(Time.deltaTime);
            }
        }
    }
}
