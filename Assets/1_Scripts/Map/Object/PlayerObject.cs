using UnityEngine;

public class PlayerObject : MovableObject
{
    protected override void Update()
    {
        base.Update();
        UpdateMoveDirection();
    }

    private void UpdateMoveDirection()
    {
        var previousMoveDirection = MoveDirection;
        MoveDirection = Vector2.zero;
        var moveUp = Input.GetKey(KeyCode.W);
        var moveDown = Input.GetKey(KeyCode.S);
        var moveRight = Input.GetKey(KeyCode.D);
        var moveLeft = Input.GetKey(KeyCode.A);
        if (moveUp && moveDown)
        {
            MoveDirection += previousMoveDirection.y > 0 ? Vector2.up : Vector2.down;
        }
        else if (moveUp)
        {
            MoveDirection += Vector2.up;
        }
        else if (moveDown)
        {
            MoveDirection += Vector2.down;
        }

        if (moveLeft && moveRight)
        {
            MoveDirection += previousMoveDirection.x > 0 ? Vector2.right : Vector2.left;
        }
        else if (moveLeft)
        {
            MoveDirection += Vector2.left;
        }
        else if (moveRight)
        {
            MoveDirection += Vector2.right;
        }

        MoveDirection.Normalize();
    }
}