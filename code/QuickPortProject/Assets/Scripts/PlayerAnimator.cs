using UnityEngine;
using Random = UnityEngine.Random;

namespace PresentationController {
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        [SerializeField] AudioSource _audioSource;
        [SerializeField] LayerMask _groundMask;
        [SerializeField] ParticleSystem _jumpParticles;
        [SerializeField] ParticleSystem _launchParticles;
        [SerializeField] ParticleSystem _landParticles;
        [SerializeField] ParticleSystem _moveParticles;
        [SerializeField] AudioClip[] _footsteps;
        [SerializeField] float _maxTilt = .1f;
        [SerializeField] float _tiltSpeed = 1.0f;
        [SerializeField, Range(1f, 3f)] float _maxIdleSpeed = 2.0f;
        [SerializeField] private float _maxParticleFallSpeed = -40;


        PlayerController _player;
        bool _bPlayerIsGrounded;
        ParticleSystem.MinMaxGradient _currentGradient;
        Vector2 _movement;


        void Awake()
        {
            _player = GetComponentInParent<PlayerController>();
        }

        void Update()
        {
            if (_player is null)
            {
                return;
            }

            FlipSprite();
            LeanIntoMovement();
            SpeedUpIdleAnim();
            HandleLanding();
            HandleJump();
        }

        void HandleJump()
        {
            if (_player.JumpingThisFrame)
            {
                _animator.SetTrigger(JumpKey);
                _animator.ResetTrigger(GroundedKey);

                // Only play particles when grounded (avoid coyote)
                if (_player.Grounded)
                {
                    SetColor(_jumpParticles);
                    SetColor(_launchParticles);
                    _jumpParticles.Play();
                }
            }
        }

        void HandleLanding()
        {
            // Splat Sound on landing
            if (_player.LandingThisFrame)
            {
                _animator.SetTrigger(GroundedKey);
                _audioSource.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
            }

            // Play landing effects and begin ground movement effects
            if (!_bPlayerIsGrounded && _player.Grounded)
            {
                _bPlayerIsGrounded = true;
                _moveParticles.Play();
                _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, _maxParticleFallSpeed, _movement.y);
                SetColor(_landParticles);
                _landParticles.Play();
            }
            else if (_bPlayerIsGrounded && !_player.Grounded)
            {
                _bPlayerIsGrounded = false;
                _moveParticles.Stop();
            }
        }

        private void SetColor(ParticleSystem particles)
        {
            var main = particles.main;
            main.startColor = _currentGradient;
        }

        private void SpeedUpIdleAnim()
        {
            _animator.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, Mathf.Abs(_player.Input.xAxis)));
        }

        void LeanIntoMovement()
        {
            var targetRotation = new Vector3(0, 0, Mathf.Lerp(-_maxTilt, _maxTilt, Mathf.InverseLerp(-1, 1, _player.Input.xAxis)));
            _animator.transform.rotation = Quaternion.RotateTowards(_animator.transform.rotation, Quaternion.Euler(targetRotation), _tiltSpeed * Time.deltaTime);
        }

        void FlipSprite()
        {
            if (_player.Input.xAxis != 0)
            {
                transform.localScale = new Vector3(_player.Input.xAxis > 0 ? 1 : -1, 1, 1);
            }
        }

        void OnEnable()
        {
            _moveParticles.Play();
        }

        void OnDisable()
        {
            _moveParticles.Stop();
        }


        #region Animation Keys
        private static readonly int GroundedKey = Animator.StringToHash("Grounded");
        private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
        private static readonly int JumpKey = Animator.StringToHash("Jump");
        #endregion
    }
}

