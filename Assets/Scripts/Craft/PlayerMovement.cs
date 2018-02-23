using Assets.Scripts.Common;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Craft
{
    public enum JumpState
    {
        None,
        Jump,
        Jetpack,
        InAir
    }

    public class PlayerMovement : MonoBehaviour
    {
        public GameObject PlayerParent;
        public GameObject BodyCollider;
        public GameObject CeilingCheck;
        public GameObject JetpackBoostBar;
        public GameObject JetpackLeft;
        public GameObject JetpackRight;
        public GameObject LeftFoot;
        public GameObject RightFoot;

        public float Gravity;
        public float GravityIncreaseRate;
        public float GroundCollisionCheckRadius;
        public float CeilingCollisionCheckRadius;

        public float JetPackStrength;
        public float JetPackGas;
        public float MaxGasVolume;
        public float UseRate;
        public float RefillDelay;
        public float RefillRate;

        public float JumpForce;
        public float MaxJumpDuration;
        public float MinJumpDuration;

        public float AirSpeed;
        public float RunForwardSpeed;
        public float RunBackwardSpeed;
        public float JetpackSpeed;
        public float TurnSpeed;

        public bool Grounded { get; set; }

        private readonly int _backflipState = Animator.StringToHash("Base.Backflip");
        private readonly int _idleState = Animator.StringToHash("Base.Idle");
        private readonly int _inAir = Animator.StringToHash("Base.InAir");
        private readonly int _jetpackState = Animator.StringToHash("Base.Jetpack");
        private readonly int _runState = Animator.StringToHash("Base.Run");

        private JumpState _jumpState;
        private Animator _animator;
        private CollisionDetector _bodyCollisionDetector;
        private Vector3 _moveDirection = Vector3.zero;
        private float _currentJumpDuration;
        private float _currentRefillDelay;
        private float _gravityAcceleration;
        private float _jumpForce;
        private bool _jumping;
        private bool _refillDelayActive;
        private int _animatorState;
        private ParticleSystem.EmissionModule _rightJetpackEmissionModule;
        private ParticleSystem.EmissionModule _leftJetpackEmissionModule;
        private Vector3 _worldPos;
        private bool _facingRight;
        private bool _movingForward;
        private InputManager _inputManager;

        public void Awake()
        {
            _animator = GetComponent<Animator>();
            _inputManager = GetComponent<InputManager>();
            Setup();
        }

        public void Start()
        {
            _jumpForce = JumpForce;
            _currentJumpDuration = 0;
            _jumpState = JumpState.None;
        }

        public void Setup()
        {
            _leftJetpackEmissionModule = JetpackLeft.GetComponent<ParticleSystem>().emission;
            _rightJetpackEmissionModule = JetpackRight.GetComponent<ParticleSystem>().emission;
            _bodyCollisionDetector = BodyCollider.GetComponent<CollisionDetector>();
            _currentRefillDelay = RefillDelay;
        }

        private void Update()
        {
            _animator.SetBool("Jetpack", false);
            _animator.SetBool("Jump", false);
            _animator.SetFloat("Direction", 0);

            if (_inputManager.GetInputAvailable() == false)
            {
                _facingRight = true;
                gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.localEulerAngles.x, 90, gameObject.transform.localEulerAngles.z);
                _moveDirection = new Vector3();
                _jumping = false;
                _jumpState = JumpState.None;
                return;
            }

            _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            DeactivateJetpackParticle();
            var jumpPressed = false;
            if (!Grounded && !(_jumpState == JumpState.Jump || _jumpState == JumpState.Jetpack))
            {
                _jumpState = JumpState.InAir;
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (_jumpState == JumpState.None)
                {
                    _jumpState = JumpState.Jump;
                }
                else if (_jumpState == JumpState.Jump)
                {
                    _jumpState = JumpState.Jetpack;
                } else if (_jumpState == JumpState.InAir)
                {
                    _jumpState = JumpState.Jetpack;
                }
            }

            if (_jumpState == JumpState.Jump)
            {
                _currentJumpDuration += MaxJumpDuration*Time.deltaTime;
            }

            if (Input.GetButton("Jump"))
            {
                jumpPressed = true;
            }

            if (_jumpState == JumpState.Jump && _currentJumpDuration <= MinJumpDuration)
            {
                Jump(_currentJumpDuration, 0, MinJumpDuration, 0, 1 / MaxJumpDuration * MinJumpDuration, JumpForce);
            }
            else if (_jumpState == JumpState.Jump && jumpPressed && _currentJumpDuration >= MinJumpDuration &&
                     _currentJumpDuration < MaxJumpDuration)
            {
                Jump(_currentJumpDuration, MinJumpDuration, MaxJumpDuration, 1/MaxJumpDuration*MinJumpDuration, 1, JumpForce);
            }
            else
            {
                _jumping = false;
            }

            if (_jumpState == JumpState.Jetpack && jumpPressed && JetPackGas > 1)
            {
                ActivateJetpackParticle();
                _currentRefillDelay = RefillDelay;
                _refillDelayActive = true;
                _gravityAcceleration = 0;
                _animator.SetBool("Jetpack", true);
                _jumping = false;
                _animator.SetBool("Jump", false);
            }
            else if (Grounded && !_jumping)
            {
                _jumpForce = 0;
                _jumpState = JumpState.None;
                _currentJumpDuration = 0;
                _jumping = false;
                _animator.SetBool("Jump", false);
            }

            JetpackBoostBar.GetComponent<UiBar>().SetValue(Mathf.Clamp(JetPackGas, 0, MaxGasVolume), MaxGasVolume);
            RefillJetpackGasoline();
            _movingForward = false;
            if (_moveDirection.x > 0)
            {
                _movingForward = _facingRight;
            } else if (_moveDirection.x < 0)
            {
                _movingForward = !_facingRight;
            }
            _animator.SetBool("Forward", _movingForward); 
            _animator.SetFloat("Direction", Mathf.Abs(_moveDirection.x));
            _animator.SetBool("Backflip", Input.GetButton("Backflip"));
        }

        private void Jump(float curDur, float iStart, float iEnd, float oStart, float oEnd, float force)
        {
            _jumping = true;
            _animator.SetBool("Jump", true);
            _gravityAcceleration = 0;
            var t = GameObjectHelper.Map(curDur, iStart, iEnd, oStart, oEnd);

            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            _jumpForce = Mathf.Lerp(force, 0, t);
        }

        private void RefillJetpackGasoline()
        {
            if (_animatorState == _jetpackState) return;

            if (_refillDelayActive)
            {
                if (_currentRefillDelay >= 0)
                {
                    _currentRefillDelay -= Time.deltaTime;
                }
                else
                {
                    _refillDelayActive = false;
                }
            }
            else
            {
                JetPackGas += RefillRate*Time.deltaTime;
                if (JetPackGas > MaxGasVolume)
                {
                    JetPackGas = MaxGasVolume;
                }
            }
        }

        private void FixedUpdate()
        {
            CheckIfGrounded();

            var transform = gameObject.transform;
            _animatorState = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            var speed = _movingForward ? RunForwardSpeed : RunBackwardSpeed;

            //Gravity Simulation
            if (!Grounded)
            {
                _gravityAcceleration += Time.deltaTime/GravityIncreaseRate;
                if (_gravityAcceleration > Gravity)
                {
                    _gravityAcceleration = Gravity;
                }
                transform.position = Vector3.Lerp(transform.position,
                    new Vector3(transform.position.x, transform.position.y - _gravityAcceleration, transform.position.z),
                    _gravityAcceleration);
            }

            var mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = -Camera.main.transform.position.z;

            var cameraPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            _worldPos = Vector3.Lerp(_worldPos, new Vector3(cameraPos.x, cameraPos.y, 0), Time.deltaTime * TurnSpeed);

            //Player facing
            if (_worldPos.x < gameObject.transform.position.x)
            {
                _facingRight = false;
                gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.localEulerAngles.x, 270, gameObject.transform.localEulerAngles.z);
            } else if (_worldPos.x > gameObject.transform.position.x)
            {
                _facingRight = true;
                gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.localEulerAngles.x, 90, gameObject.transform.localEulerAngles.z);
            }
            
            if (_animatorState == _runState)
            {
                DeactivateJetpackParticle();
                speed = RunForwardSpeed;
            }
            else if (_animatorState == _idleState)
            {
                DeactivateJetpackParticle();
            }
            else if (_animatorState == _jetpackState)
            {
                JetPackGas -= UseRate*Time.deltaTime;
                if (JetPackGas <= 0)
                {
                    JetPackGas = 0;
                }
                if (JetPackGas > 0)
                {
                    MoveVertical(JetPackStrength);
                }
                Grounded = false;
                speed = JetpackSpeed;
            }
            else if (_animatorState == _backflipState)
            {
            }
            else if (_animatorState == _inAir)
            {
                DeactivateJetpackParticle();
                speed = AirSpeed;
            }

            //Jump
            if (_jumping && _animatorState != _jetpackState)
            {
                _currentJumpDuration += Time.deltaTime;


                MoveVertical(_jumpForce);
                speed = _movingForward ? RunForwardSpeed : RunBackwardSpeed;
            }


            MoveHorizontal(speed);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        private void MoveVertical(float up)
        {
            var colliders = Physics2D.OverlapCircleAll(CeilingCheck.transform.position, CeilingCollisionCheckRadius);
            var canGoUp = true;
            foreach (var coll in colliders)
            {
                if (coll.gameObject.layer != LayerMask.NameToLayer("Player"))
                {
                    canGoUp = false;
                }
            }
            if (canGoUp)
            {
                gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + up,
                    transform.position.z);
            }
        }

        private void MoveHorizontal(float speed)
        {
            if (_bodyCollisionDetector.CollidesRight)
            {
                if ((_moveDirection.x < 0 || _moveDirection.x > 0) && _movingForward)
                {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, gameObject.transform.position + _moveDirection, Time.deltaTime * speed);
                }
              
            } else if (_bodyCollisionDetector.CollidesLeft)
            {
                if ((_moveDirection.x < 0 || _moveDirection.x > 0) && !_movingForward)
                {
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, gameObject.transform.position + _moveDirection, Time.deltaTime*speed);
                }
            }
            else
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, gameObject.transform.position + _moveDirection, Time.deltaTime * speed);
            }
        }

        private void CheckIfGrounded()
        {
            Grounded = false;
            _animator.SetBool("Grounded", false);
            gameObject.transform.parent = PlayerParent.transform.root;

            var playerCenter = Vector3.Lerp(LeftFoot.transform.position, RightFoot.transform.position, 0.5f);
            var bottom = 0f;
            if (LeftFoot.transform.position.y < RightFoot.transform.position.y)
            {
                bottom = LeftFoot.transform.position.y;
            }
            else
            {
                bottom = RightFoot.transform.position.y;
            }

            var colliders = Physics2D.OverlapCircleAll(new Vector3(playerCenter.x, bottom, playerCenter.z), GroundCollisionCheckRadius);
            foreach (var col in colliders)
            {
                GroundCollision(col);
            }
        }

        void LateUpdate()
        {
           
        }

        private void GroundCollision(Collider2D col)
        {
            if (col.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                if (!_jumping && _animatorState != _jetpackState)
                {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                        col.bounds.max.y, gameObject.transform.position.z);
                }
                Grounded = true;
                _animator.SetBool("Grounded", true);
                _gravityAcceleration = 0;
            }

            if (col.gameObject.layer == LayerMask.NameToLayer("movablePlatform"))
            {
                gameObject.transform.parent = col.gameObject.transform;
            }
        }

        private void ActivateJetpackParticle()
        {
            _leftJetpackEmissionModule.enabled = true;
            _rightJetpackEmissionModule.enabled = true;
        }

        private void DeactivateJetpackParticle()
        {
            _leftJetpackEmissionModule.enabled = false;
            _rightJetpackEmissionModule.enabled = false;
        }
    }
}