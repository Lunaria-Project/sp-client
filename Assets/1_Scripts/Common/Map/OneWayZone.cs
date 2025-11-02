using UnityEngine;

public class OneWayZone : MapTrigger
{
    [SerializeField] private Vector2 _moveDirection;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent<MovableObject>(out var movableObject)) return;
        var moveDirection = movableObject.MoveDirection;
        if (moveDirection == Vector2.zero)
        {
            movableObject.SetForceMoveDirection(Vector2.zero);
        }
        else if (moveDirection.x < 0 || moveDirection.y > 0)
        {
            movableObject.SetForceMoveDirection(_moveDirection.normalized);
        }
        else if (moveDirection.x > 0 || moveDirection.y < 0)
        {
            movableObject.SetForceMoveDirection(-_moveDirection.normalized);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<MovableObject>(out var movableObject)) return;
        movableObject.SetForceMoveDirection(Vector2.zero);
    }
}