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

    public int maxHealth = 500;
    public int currentHealth = 500;
    


    private CharacterController characterController;
    private CinemachineCamera cinemachineCamera;
    private Vector2 move;
    private Vector3 velocity;
    public bool fire;

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








    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameManager.Instance.GameOver();
    }

    public void DealDamage(EnemyManager target, int damage)
    {
        target.TakeDamage(damage);
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

    public void OnAttack(InputValue inputValue)
    {
        fire = inputValue.Get<float>() > 0.5f ? true : false;
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