using UnityEngine;

public class MyhomeMapManager : MonoBehaviour
{
    [SerializeField] private PlayerObject _player;

    private void Update()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        var input = new Vector2(x, y);
        if (input.sqrMagnitude > 1f)
        {
            input = input.normalized;
        }

        _player.SetInput(input);
    }
}