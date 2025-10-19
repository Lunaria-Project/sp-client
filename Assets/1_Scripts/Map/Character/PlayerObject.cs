using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : CharacterObject
{
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Collider2D _collider2D;
    
    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;          // units/sec
    [SerializeField] private float shellRadius = 0.01f;     // 겹침 방지 보정
    [SerializeField] private int maxSlideIterations = 3;    // 반복 충돌 처리 횟수

    [Header("Collision Filter")]
    [SerializeField] private ContactFilter2D contactFilter; // 충돌 레이어/각도 설정

    // 입력 누적
    private Vector2 _input;

    private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[8];
    private readonly List<RaycastHit2D> _hitBufferList = new (8);

    // 추가: 겹침 해소용 버퍼와 미세 푸시
    private readonly Collider2D[] _overlapBuffer = new Collider2D[8];
    private const float TinyPush = 0.0005f;

    private void Awake()
    {
        // 권장 설정: Kinematic + Continuous
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        LogManager.Log("CharacterManager ready");
    }

    private void Update()
    {
        // 간단 입력. 필요시 외부에서 SetInput 호출 가능.
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");
        _input = new Vector2(x, y);
        if (_input.sqrMagnitude > 1f)
        {
            _input.Normalize();
        }
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

    /// <summary>
    /// 외부 시스템(가상 조이스틱 등)에서 입력 주입용
    /// </summary>
    public void SetInput(Vector2 input)
    {
        _input = input.sqrMagnitude > 1f ? input.normalized : input;
    }

    private void MoveAndSlide(Vector2 delta)
    {
        // 남은 이동량을 반복 처리. 충돌 시 노멀 방향 성분을 제거하여 벽을 따라 슬라이드.
        var remaining = delta;
        for (var i = 0; i < maxSlideIterations; i++)
        {
            if (remaining == Vector2.zero)
            {
                return;
            }

            // 캐스트로 예상 충돌 탐지
            var distance = remaining.magnitude;
            var count = _rigidbody2D.Cast(remaining.normalized, contactFilter, _hitBuffer, distance + shellRadius);

            _hitBufferList.Clear();
            for (var h = 0; h < count; h++)
            {
                _hitBufferList.Add(_hitBuffer[h]);
            }

            if (_hitBufferList.Count == 0)
            {
                // 충돌 없음. 전부 이동.
                _rigidbody2D.position += remaining;
                Depenetrate(); // 이동 후 혹시 모를 겹침 해소
                return;
            }

            // 가장 가까운 히트 선택
            var minT = 1f;
            var bestHit = _hitBufferList[0];
            for (var h = 0; h < _hitBufferList.Count; h++)
            {
                var hit = _hitBufferList[h];
                // t ≈ (hit.distance - shell)/distance
                var t = Mathf.Clamp01((hit.distance - shellRadius) / distance);
                if (t < minT)
                {
                    minT = t;
                    bestHit = hit;
                }
            }

            // 가능한 만큼 전진
            var advance = remaining.normalized * (distance * minT);
            _rigidbody2D.position += advance;
            Depenetrate(); // 곡면/모서리 끼임 방지

            // 남은 이동에서 노멀 방향 성분 제거 => 벽을 따라 슬라이드
            var n = bestHit.normal;
            var v = remaining - advance;
            var slide = v - Vector2.Dot(v, n) * n;
            slide += n * TinyPush; // 바깥으로 아주 살짝 띄워 재침투 방지

            // 미세 진동 방지: 아주 작으면 종료
            if (slide.sqrMagnitude < 1e-6f)
            {
                return;
            }

            remaining = slide;
        }
    }

    // 추가: 겹침 탈출
    private void Depenetrate()
    {
        var count = _collider2D.OverlapCollider(contactFilter, _overlapBuffer);
        for (var i = 0; i < count; i++)
        {
            var other = _overlapBuffer[i];
            if (other == null)
            {
                continue;
            }

            var d = _collider2D.Distance(other); // 내 콜라이더 기준 분리 정보
            if (d.isOverlapped)
            {
                _rigidbody2D.position += d.normal * (d.distance + shellRadius * 0.5f);
            }
        }
    }
}
