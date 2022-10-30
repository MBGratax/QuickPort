using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace PresentationController
{
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        public Vector3 Velocity { get; private set; }
        public FrameInput Input { get => _polledInput; private set => _initialInput = value; }
        public bool JumpingThisFrame { get; private set; }
        public bool LandingThisFrame { get; private set; }
        public Vector3 RawMovement { get; private set; }
        public bool Grounded => _bIsCollidingDown;

        Vector3 _lastPosition;
        float _currentHorizontalSpeed;
        float _currentVerticalSpeed;
        [SerializeField] PlayerInput playerInput;
        GameControls inputActions;
        bool _bIsActive;
        FrameInput _initialInput;
        FrameInput _polledInput;

        /// <summary>
        /// Enable inputactions and make sure the object is alive (Colliders not initialized on first frame).
        /// Cache playerinput component.
        /// </summary>
        void Awake()
        {
            Invoke(nameof(Activate), 0.5f);
            playerInput = GetComponent<PlayerInput>();
            inputActions = new GameControls();
            inputActions.Player.Enable();
        }

        public void Activate()
        {
            _bIsActive = true;
        }

        public void ButtonStateEventTest(bool value)
        {
            QuickPortAPI.QuickPortLogger.QPLog(this, $"Button State: {value}");
        }
        public void ButtonDownEventTest()
        {
            QuickPortAPI.QuickPortLogger.QPLog(this, "Button Down");
        }
       public void ButtonUpEventTest()
        {
            QuickPortAPI.QuickPortLogger.QPLog(this, "Button Up");
        }

        public void JoystickRawInputEventTest(Vector2 value)
        {
            QuickPortAPI.QuickPortLogger.QPLog(this, $"Raw Input: {value}");
        }

        public void JoystickProcessedInputEventTest(Vector2 value)
        {
            QuickPortAPI.QuickPortLogger.QPLog(this, $"Processed Input: {value}");
        }

        void Update()
        {
            // Abort very first frame to avoid NREs from colliders
            if (!_bIsActive)
            {
                return;
            }

            // Calculate Velocity
            Velocity = (transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;

            // Add X Axis input to polled Input
            _initialInput.xAxis = inputActions.Player.Move.ReadValue<Vector2>().x;

            PollInput();
            CheckCollisions();

            CalculateHorizontalMovement();
            CalculateJumpApexModifier();
            CalculateVerticalMovement();
            CalculateJumpMovement();

            MoveCharacter();
        }

        #region Poll Input

        void PollInput()
        {
            _polledInput = new FrameInput
            {
                bJumpPressed = _initialInput.bJumpPressed,
                bJumpReleased = _initialInput.bJumpReleased,
                xAxis = _initialInput.xAxis
            };
        }

        void OnJump(CallbackContext context)
        {
            _initialInput.bJumpPressed = context.performed;
            _initialInput.bJumpReleased = context.canceled;
            if (context.performed)
            {
                _lastJumpPressed = Time.time;
            }
        }

        #endregion

        #region Collisions

        [Header("Collision")]
        [SerializeField] Bounds _characterBounds;
        [SerializeField] LayerMask _groundLayer;
        [SerializeField] int _detectorCount = 3;
        [SerializeField] float _detectionRayLength = 0.1f;
        // This prevents the side rays from detecting collision on the ground
        [SerializeField] [Range(0.1f, 0.3f)] float _rayBuffer = 0.1f;

        RayRange _raysUp;
        RayRange _raysRight;
        RayRange _raysDown;
        RayRange _raysLeft;

        bool _bIsCollidingUp;
        bool _bIsCollidingRight;
        bool _bIsCollidingDown;
        bool _bIsCollidingLeft;
        bool _bIsGrounded;

        float _timeLeftGrounded;

        /// <summary>
        /// Uses Raycasts to check for collision.
        /// </summary>
        void CheckCollisions()
        {
            CalculateRayRanges();

            // Groundchecks
            LandingThisFrame = false;
            _bIsGrounded = RunDetection(_raysDown);
            if (_bIsCollidingDown && !_bIsGrounded)
            {
                _timeLeftGrounded = Time.time;
            }
            else if (!_bIsCollidingDown && _bIsGrounded)
            {
                _bCoyoteTimeUsable = true;
                LandingThisFrame = true;
            }

            _bIsCollidingDown = _bIsGrounded;

            //Check Collision in other Directions
            _bIsCollidingUp = RunDetection(_raysUp);
            _bIsCollidingRight = RunDetection(_raysRight);
            _bIsCollidingLeft = RunDetection(_raysLeft);
        }

        void CalculateRayRanges()
        {
            var bounds = new Bounds(transform.position, _characterBounds.size);

            _raysDown = new RayRange(bounds.min.x + _rayBuffer, bounds.min.y, bounds.max.x - _rayBuffer, bounds.min.y, Vector2.down);
            _raysUp = new RayRange(bounds.min.x + _rayBuffer, bounds.max.y, bounds.max.x - _rayBuffer, bounds.max.y, Vector2.up);
            _raysLeft = new RayRange(bounds.min.x, bounds.min.y + _rayBuffer, bounds.min.x, bounds.max.y - _rayBuffer, Vector2.left);
            _raysRight = new RayRange(bounds.max.x, bounds.min.y + _rayBuffer, bounds.max.x, bounds.max.y - _rayBuffer, Vector2.right);
        }

        bool RunDetection(RayRange ray)
        {
            return EvaluateRayPosition(ray)
                .Any(point => Physics.Raycast(point, ray.Direction, _detectionRayLength, _groundLayer));
        }

        IEnumerable<Vector2> EvaluateRayPosition(RayRange ray)
        {
            for (var i = 0; i < _detectorCount; i++)
            {
                var t = (float)i / (_detectorCount - 1);
                yield return Vector2.Lerp(ray.Start, ray.End, t);
            }
        }
        void OnDrawGizmos()
        {
            // Bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

            // Rays
            if (!Application.isPlaying)
            {
                CalculateRayRanges();
                Gizmos.color = Color.blue;
                foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft })
                {
                    foreach (var point in EvaluateRayPosition(range))
                    {
                        Gizmos.DrawRay(point, range.Direction * _detectionRayLength);
                    }
                }
            }

            if (!Application.isPlaying) return;

            // Draw the future position. Handy for visualizing gravity
            Gizmos.color = Color.red;
            var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
            Gizmos.DrawWireCube(transform.position + move, _characterBounds.size);
        }

        #endregion

        #region Walk

        [Header("Movement")]
        [SerializeField] float _moveClamp = 13f;
        [SerializeField] float _acceleration = 90f;
        [SerializeField] float _deacceleration = 60f;
        [SerializeField] float _apexModifier = 2f;
        void CalculateHorizontalMovement()
        {
            if (_polledInput.xAxis != 0)
            {
                // Set Horizontal Speed
                _currentHorizontalSpeed += _polledInput.xAxis * _acceleration * Time.deltaTime;

                // Clamped by max movement per frame
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);

                // Apply Apex modifier
                var apexBonus = Mathf.Sign(_polledInput.xAxis) * _apexModifier * _apexPoint;
                _currentHorizontalSpeed += apexBonus * Time.deltaTime;
            }
            else
            {
                // No Horizontal input, slow character down
                _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deacceleration * Time.deltaTime);
            }

            if (_currentHorizontalSpeed > 0 && _bIsCollidingRight || _currentHorizontalSpeed < 0 && _bIsCollidingLeft)
            {
                // Do not run through walls
                _currentHorizontalSpeed = 0;
            }
        }

        #endregion

        #region Gravity

        [Header("Gravity")]
        [SerializeField] float _fallClamp = -40f;
        [SerializeField] float _minFallSpeed = 80f;
        [SerializeField] float _maxFallSpeed = 120f;
        float _fallSpeed;

        void CalculateVerticalMovement()
        {
            if (_bIsCollidingDown)
            {
                // Do not fall into floor
                if (_currentVerticalSpeed < 0)
                {
                    _currentVerticalSpeed = 0;
                }
            }
            else
            {
                //Add some force downward if a jump has been canceled to make controls snappy
                var fallSpeed = _bEndedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed;

                //Fall
                _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

                //Clamp Vertical speed
                if (_currentVerticalSpeed < _fallClamp)
                {
                    _currentVerticalSpeed = _fallClamp;
                }
            }
        }

        #endregion

        #region Jump

        [Header("Jumping")]
        [SerializeField] float _jumpHeight = 30f;
        [SerializeField] float _jumpApexThreshold = 10f;
        [SerializeField] float _coyoteTimeThreshold = 0.1f;
        [SerializeField] float _jumpBuffer = 0.1f;
        [SerializeField] float _jumpEndEarlyGravityModifier = 3f;
        bool _bCoyoteTimeUsable;
        bool _bEndedJumpEarly;
        float _apexPoint;
        float _lastJumpPressed;
        bool CanUseCoyote => _bCoyoteTimeUsable && !_bIsCollidingDown && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
        bool HasBufferedJump => _bIsCollidingDown && _lastJumpPressed + _jumpBuffer > Time.time;

        void CalculateJumpApexModifier()
        {
            if (!_bIsCollidingDown)
            {
                _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
                _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
            }
            else
            {
                _apexPoint = 0;
            }
        }

        void CalculateJumpMovement()
        {
            if (_polledInput.bJumpPressed && CanUseCoyote || HasBufferedJump)
            {
                _currentVerticalSpeed = _jumpHeight;
                _bEndedJumpEarly = false;
                _bCoyoteTimeUsable = false;
                _timeLeftGrounded = float.MinValue;
                JumpingThisFrame = true;
            }
            else
            {
                JumpingThisFrame = false;
            }

            // End Jump if button released
            if (!_bIsCollidingDown && _polledInput.bJumpReleased && !_bEndedJumpEarly && Velocity.y > 0)
            {
                _bEndedJumpEarly = true;
            }

            if (_bIsCollidingUp)
            {
                if (_currentVerticalSpeed > 0)
                {
                    _currentVerticalSpeed = 0;
                }
            }
        }


        #endregion

        #region Move

        [Header("Move")]
        [SerializeField, Tooltip("Increasing this value increases collision accuracy at the cost of performance.")]
        int _freeColliderIterations = 10;


        void MoveCharacter()
        {
            var pos = transform.position;
            RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed);
            var move = RawMovement * Time.deltaTime;
            var furthestPoint = pos + move;

            // Check for collision, if nothing is hit move normally
            var hit = Physics2D.OverlapBox(furthestPoint, _characterBounds.size, 0, _groundLayer);
            if (!hit)
            {
                transform.position += move;
                return;
            }

            // Otherwise move in small increments to avoid corners
            var positionToMoveTo = transform.position;
            for (int i = 1; i < _freeColliderIterations; i++)
            {
                var t = (float)i / _freeColliderIterations;
                var posToTry = Vector2.Lerp(pos, furthestPoint, t);

                if (Physics2D.OverlapBox(posToTry, _characterBounds.size, 0, _groundLayer))
                {
                    transform.position = positionToMoveTo;
                    if (i == 1)
                    {
                        if (_currentVerticalSpeed < 0)
                        {
                            _currentVerticalSpeed = 0;
                        }
                        var dir = transform.position - hit.transform.position;
                        transform.position += dir.normalized * move.magnitude;
                    }
                    return;
                }


                positionToMoveTo = posToTry;
            }

        }

        #endregion

    }
}
