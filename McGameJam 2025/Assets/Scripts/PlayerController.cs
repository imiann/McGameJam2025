using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform _camera;
    private CharacterController _characterController;
    private Vector2 _input;
    private Vector3 _direction;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 move =
            _camera.forward * _input.y + _camera.right * _input.x;
        move.y = 0;
        _characterController.Move(move * speed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }
}