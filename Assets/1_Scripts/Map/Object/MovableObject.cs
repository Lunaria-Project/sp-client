using UnityEngine;

public class MovableObject : MapObject
{
    [Header("[Move]")] [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private ContactFilter2D _contactFilter;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _frontSprites;
    [SerializeField] private Sprite[] _backSprites;
    [SerializeField] private MapConfig _config;

    public Vector2 MoveDirection { get; protected set; }
    protected Vector2 ForceMoveDirection;

    private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[8];
    private bool _isFacingFront;
    private float _spriteFrameTime;
    private int _spriteIndex;

    #region UnityEvent

    protected void Start()
    {
        _isFacingFront = true;
        InitMove();
        InitSprite();
    }

    protected virtual void Update()
    {
        UpdateSprite(Time.deltaTime);
    }

    protected void FixedUpdate()
    {
        UpdateMove(Time.fixedDeltaTime);
    }

    #endregion

    public void SetForceMoveDirection(Vector2 direction)
    {
        ForceMoveDirection = direction;
    }

    private void InitMove()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        MoveDirection = Vector2.zero;
    }

    private void InitSprite()
    {
        _spriteIndex = 0;
        if (_frontSprites.Length > 0)
        {
            _spriteRenderer.sprite = _frontSprites[_spriteIndex];
        }
        _spriteFrameTime = 0;
    }

    private void UpdateSprite(float dt)
    {
        if (_frontSprites.Length == 0 || _backSprites.Length == 0) return;

        var moveDirection = ForceMoveDirection != Vector2.zero ? ForceMoveDirection : MoveDirection;
        if (moveDirection.y > 0)
        {
            _isFacingFront = false;
        }
        else if (moveDirection.y < 0)
        {
            _isFacingFront = true;
        }

        if (moveDirection.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (moveDirection.x < 0)
        {
            _spriteRenderer.flipX = true;
        }

        if (moveDirection == Vector2.zero)
        {
            _spriteFrameTime = 0;
            _spriteIndex = 0;
        }
        else
        {
            _spriteFrameTime += dt;
            if (_spriteFrameTime > _config.FrameDuration)
            {
                _spriteFrameTime -= _config.FrameDuration;
                _spriteIndex += 1;
                _spriteIndex %= (_isFacingFront ? _frontSprites.Length : _backSprites.Length);
            }
        }

        _spriteRenderer.sprite = _isFacingFront ? _frontSprites[_spriteIndex] : _backSprites[_spriteIndex];
    }

    private void UpdateMove(float dt)
    {
        var moveDirection = ForceMoveDirection != Vector2.zero ? ForceMoveDirection : MoveDirection;
        if (moveDirection == Vector2.zero) return;

        var deltaPosition = moveDirection * (dt * _config.PlayerSpeed);
        for (var i = 0; i < _config.CollisionResolveCount; i++)
        {
            var deltaDistance = deltaPosition.magnitude;
            var collisionCount = _rigidbody2D.Cast(deltaPosition.normalized, _contactFilter, _hitBuffer, deltaDistance + _config.CollisionMargin);
            if (collisionCount == 0)
            {
                _rigidbody2D.position += deltaPosition;
                return;
            }

            var minCollisionRatio = 1f;
            var nearestHit = _hitBuffer[0];
            for (var j = 0; j < collisionCount; j++)
            {
                var hit = _hitBuffer[j];
                var collisionRatio = Mathf.Clamp01((hit.distance - _config.CollisionMargin) / deltaDistance);
                if (collisionRatio < minCollisionRatio)
                {
                    minCollisionRatio = collisionRatio;
                    nearestHit = hit;
                }
            }

            var newDeltaPosition = deltaPosition * minCollisionRatio;
            _rigidbody2D.position += newDeltaPosition;

            // 남은 이동에서 법선 성분 제거하여 슬라이드
            var normal = nearestHit.normal;
            deltaPosition *= (1 - minCollisionRatio);
            var slide = deltaPosition - Vector2.Dot(deltaPosition, normal) * normal;
            slide += normal * _config.SlidePush;

            if (slide.magnitude <= float.Epsilon) return;

            deltaPosition = slide;
        }
    }
}