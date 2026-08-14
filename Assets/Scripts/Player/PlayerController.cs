using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float currentCharacterSpeed;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 2f;
    public float gravity = -50f;



    private CharacterController characterController;
    private CinemachineCamera cinemachineCamera;
    private Vector2 move;
    private Vector3 velocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
        currentCharacterSpeed = walkSpeed;
    }

    private void Update()
    {
        // characterController.isGrounded - встроенная проверка!
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Горизонтальное движение
        Vector3 movement = (GetForward() * move.y + GetRight() * move.x) * currentCharacterSpeed;

        // Гравитация (CharacterController не применяет её автоматически при использовании Move)
        velocity.y += gravity * Time.deltaTime;

        // Двигаем с учётом вертикали
        characterController.Move((movement + velocity) * Time.deltaTime);
    }

    public void OnMove(InputValue inputValue)
    {
        move = inputValue.Get<Vector2>();
    }

    public void OnSprint(InputValue inputValue)
    {
        currentCharacterSpeed = inputValue.Get<float>() > 0.5f ? runSpeed : walkSpeed;
    }

    public void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed && characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private Vector3 GetForward()
    {
        Vector3 forward = cinemachineCamera.transform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    private Vector3 GetRight()
    {
        Vector3 right = cinemachineCamera.transform.right;
        right.y = 0f;
        return right.normalized;
    }
}










//using Unity.Cinemachine;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.Rendering;

//public class PlayerController : MonoBehaviour
//{

//    [Header("Speed")]
//    public float currentCharacterSpeed;
//    public float walkSpeed = 5f;
//    public float runSpeed = 8f;

//    [Header("Jump")]
//    private bool isJumping = false;
//    public float jumpHeight = 2f;
//    public float gravity = -5f;



//    private CharacterController characterController;
//    private CinemachineCamera cinemachineCamera;


//    private Vector2 move;


//    private void Awake()
//    {
//        characterController = GetComponent<CharacterController>();
//        cinemachineCamera = GetComponentInChildren<CinemachineCamera>();

//        currentCharacterSpeed = walkSpeed;
//    }

//    private void Update()
//    {
//        // SimpleMove автоматически применяет гравитацию
//        // и проверяет grounded через characterController.isGrounded
//        Vector3 direction = (GetForward() * move.y + GetRight() * move.x).normalized;

//        if (isJumping && characterController.isGrounded)
//        {
//            // Для прыжка через SimpleMove нужно добавить вертикальную скорость
//            // Но SimpleMove не позволяет контролировать высоту прыжка напрямую
//            // Поэтому лучше использовать Move (см. Вариант 2)
//            isJumping = false;
//        }

//        characterController.SimpleMove(direction * currentCharacterSpeed);
//    }

//    public void OnMove(InputValue inputValue)
//    {
//        move = inputValue.Get<Vector2>();
//    }

//    public void OnSprint(InputValue inputValue)
//    {
//        if (inputValue.Get<float>() > 0.5f)
//        {
//            currentCharacterSpeed = runSpeed;
//        }
//        else
//        {
//            currentCharacterSpeed = walkSpeed;
//        }
//    }

//    private Vector3 GetForward()
//    {
//        Vector3 forward = cinemachineCamera.transform.forward;
//        forward.y = 0f;
//        return forward.normalized;
//    }

//    private Vector3 GetRight()
//    {
//        Vector3 right = cinemachineCamera.transform.right;
//        right.y = 0f;
//        return right.normalized;
//    }
//}
