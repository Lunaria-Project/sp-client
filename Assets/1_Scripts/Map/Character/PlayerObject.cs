using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : CharacterObject
{
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _frontSprites;
    [SerializeField] private Sprite[] _backSprites;

    [Header("Move")] [SerializeField] private float moveSpeed = 5f; // units/sec
    [SerializeField] private float shellRadius = 0.01f; // 겹침 방지 보정
    [SerializeField] private int maxSlideIterations = 3; // 반복 충돌 처리 횟수

    [Header("Animation")] [SerializeField] private float frameDuration = 0.12f; // 프레임당 시간

    [Header("Collision Filter")] [SerializeField]
    private ContactFilter2D contactFilter; // 충돌 레이어/각도 설정

    private Vector2 _input;

    private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[8];
    private readonly List<RaycastHit2D> _hitBufferList = new(8);
    private readonly Collider2D[] _overlapBuffer = new Collider2D[8];
    private const float TinyPush = 0.0005f;

    private enum Facing
    {
        Front,
        Back
    }

    private Facing _facing = Facing.Front;
    private int _frameIndex;
    private float _frameTimer;
    private bool _isMoving;

    private void Awake()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (_spriteRenderer != null && _frontSprites.Length > 0)
        {
            _spriteRenderer.sprite = _frontSprites[0];
        }

        LogManager.Log("CharacterManager ready");
    }

    private void Update()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");
        _input = new Vector2(x, y);
        if (_input.sqrMagnitude > 1f)
        {
            _input.Normalize();
        }

        UpdateSpriteAnimation(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        if (_input == Vector2.zero)
        {
            return;
        }

        var delta = _input * moveSpeed * dt;
        MoveAndSlide(delta);
    }

    public void SetInput(Vector2 input)
    {
        _input = input.sqrMagnitude > 1f ? input.normalized : input;
    }

    private void MoveAndSlide(Vector2 delta)
    {
        var remaining = delta;
        for (var i = 0; i < maxSlideIterations; i++)
        {
            if (remaining == Vector2.zero)
            {
                return;
            }

            var distance = remaining.magnitude;
            var count = _rigidbody2D.Cast(remaining.normalized, contactFilter, _hitBuffer, distance + shellRadius);

            _hitBufferList.Clear();
            for (var h = 0; h < count; h++)
            {
                _hitBufferList.Add(_hitBuffer[h]);
            }

            if (_hitBufferList.Count == 0)
            {
                _rigidbody2D.position += remaining;
                Depenetrate();
                return;
            }

            var minT = 1f;
            var bestHit = _hitBufferList[0];
            for (var h = 0; h < _hitBufferList.Count; h++)
            {
                var hit = _hitBufferList[h];
                var t = Mathf.Clamp01((hit.distance - shellRadius) / distance);
                if (t < minT)
                {
                    minT = t;
                    bestHit = hit;
                }
            }

            var advance = remaining.normalized * (distance * minT);
            _rigidbody2D.position += advance;
            Depenetrate();

            var n = bestHit.normal;
            var v = remaining - advance;
            var slide = v - Vector2.Dot(v, n) * n;
            slide += n * TinyPush;

            if (slide.sqrMagnitude < 1e-6f)
            {
                return;
            }

            remaining = slide;
        }
    }

    private void Depenetrate()
    {
        var count = _collider2D.Overlap(contactFilter, _overlapBuffer);
        for (var i = 0; i < count; i++)
        {
            var other = _overlapBuffer[i];
            if (other == null)
            {
                continue;
            }

            var d = _collider2D.Distance(other);
            if (d.isOverlapped)
            {
                _rigidbody2D.position += d.normal * (d.distance + shellRadius * 0.5f);
            }
        }
    }

    private void UpdateSpriteAnimation(float dt)
    {
        _isMoving = _input.sqrMagnitude > 0.0001f;

        if (_input.y > 0.1f)
        {
            _facing = Facing.Back;
        }
        else if (_input.y < -0.1f)
        {
            _facing = Facing.Front;
        }

        if (_spriteRenderer == null)
        {
            return;
        }

        var frames = _facing == Facing.Front ? _frontSprites : _backSprites;
        if (frames == null || frames.Length == 0)
        {
            return;
        }

        if (_isMoving)
        {
            _frameTimer += dt;
            if (_frameTimer >= frameDuration)
            {
                _frameTimer -= frameDuration;
                _frameIndex = (_frameIndex + 1) % frames.Length;
            }
        }
        else
        {
            _frameIndex = 0;
            _frameTimer = 0f;
        }

        _spriteRenderer.sprite = frames[_frameIndex];

        if (_input.x < 0f)
        {
            _spriteRenderer.flipX = true;
        }
        else if (_input.x >= 0f)
        {
            _spriteRenderer.flipX = false;
        }
    }
}