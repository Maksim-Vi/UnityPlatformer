using System.Threading.Tasks;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Platformer
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] Rigidbody _rb;
        [SerializeField, Self] Animator _animatior;
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

        Transform _mainCamera;
        const float ZeroF = 0f;
        float currentSpeed = 0f;
        float velocity = 0f;
        float jumpVelocity = 0f;

        Vector3 movement;

        // List<Timer> timers;
        // CountdownTimer jumpTimer;
        // CountdownTimer jumpCooldownTimer;

        bool isStartJumpung = false;
        bool isStartedJumpung = false;

        StateMachine _stateMachine;
        RunState _runState;
        JumpState _jumpState;

        static readonly int speedAnimation = Animator.StringToHash("Speed");

        private void Awake() 
        {
            _mainCamera = Camera.main.transform;
            _freeLookVCam.Follow = transform;
            _freeLookVCam.LookAt = transform;
            _freeLookVCam.OnTargetObjectWarped(transform, transform.position - _freeLookVCam.transform.position - Vector3.forward);

            _rb.freezeRotation = true;

            //State Machine
            _stateMachine = new StateMachine();

            //initStates
            _runState = new RunState(this, _animatior);
            _jumpState = new JumpState(this, _animatior);

            //Define transition
            At(_jumpState, _runState, new FuncPredicateBase(() => isStartJumpung));
            At(_runState, _jumpState, new FuncPredicateBase(() => _groundChecker.IsGround && !isStartJumpung));

            _stateMachine.SetState(_runState);
        }

        private void Start()
        {
            _input.EnablePlayerActions();
        }

        private void OnEnable() 
        {
            _input.Jump += OnJump;
        }

        private void OnDisable() 
        {
            _input.Jump -= OnJump;
        }

        private void Update() 
        {
            movement = new Vector3(_input.Direction.x, 0f, _input.Direction.y);
            
            _stateMachine.Update();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        private void OnJump(bool isJumped)
        {
            if(isJumped && _groundChecker.IsGround)
            {
                isStartJumpung = true;
                jumpVelocity = _jumpForce;
                // HandleJump();
            }
        }

        public void HandleIdle()
        {
            SmoothSpeed(ZeroF);
            UpdateAnimator(0f);
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
                SmoothSpeed(adjustedDirection.magnitude);

                UpdateAnimator(0.5f);
            }
            else {
              HandleIdle();
            }
        }

        public async void HandleJump()
        {
            if(!isStartJumpung && _groundChecker.IsGround)
            { 
                jumpVelocity = ZeroF;
                isStartJumpung = false;
                isStartedJumpung = false;
                // jumpTimer.Stop();
                return;
            }

            if(!isStartedJumpung){
                isStartJumpung = false;
                isStartedJumpung = true;

                jumpVelocity += Physics.gravity.y * _gravityMultiplayer * Time.fixedDeltaTime;
                _rb.velocity = new Vector3(_rb.velocity.x, jumpVelocity, _rb.velocity.z);

                await UniTask.Delay(50);
                isStartedJumpung = false;
            }
        }

        private void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            Vector3 velocity = adjustedDirection * _moveSpeed * Time.deltaTime;
            _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z);
        }

        private void HandleRotation(Vector3 adjustedDirection)
        {
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _speedRotation * Time.deltaTime);
            transform.LookAt(transform.position + adjustedDirection);
        }

        private void UpdateAnimator(float val)
        {
            _animatior.SetFloat("Speed", val);
        }

        void SmoothSpeed(float val)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, val, ref velocity, _smoothTime);
        }

        // private void HandleTimer()
        // {
        //     foreach (var timer in timers)
        //     {
        //         if(Time.deltaTime == 0f){
        //             Debug.Log("timer");
        //         }
        //         timer.Tick(Time.deltaTime);
        //     }
        // }
    }
}
