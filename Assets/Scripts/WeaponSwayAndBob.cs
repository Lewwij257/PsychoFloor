using UnityEngine;

public class WeaponSwayAndBob : MonoBehaviour
{
    [Header("Idle Sway (лёгкое покачивание в руках)")]
    public float idleSwayAmount = 0.015f;
    public float idleSwaySpeed = 1.2f;

    [Header("Bobbing (покачивание при движении)")]
    public float walkBobAmount = 0.04f;
    public float runBobAmount = 0.09f;
    public float walkBobSpeed = 8f;
    public float runBobSpeed = 14f;

    [Header("Общие настройки")]
    public float smooth = 8f;
    public float horizontalMultiplier = 1.4f;
    public float minSpeedToBob = 0.15f;          // минимальная скорость, чтобы начался bob

    [Header("Ссылки")]
    public PlayerController playerController;    // твой контроллер
    public CharacterController characterController;

    private Vector3 initialPosition;
    private float timer;
    private float currentBobAmount;
    private float currentBobSpeed;

    void Start()
    {
        initialPosition = transform.localPosition;

        // Автоматический поиск, если не назначил в инспекторе
        if (playerController == null)
            playerController = GetComponentInParent<PlayerController>();

        if (characterController == null)
            characterController = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        if (playerController == null || characterController == null)
            return;

        // Горизонтальная скорость
        Vector3 horizontalVelocity = new Vector3(
            characterController.velocity.x,
            0f,
            characterController.velocity.z
        );
        float speed = horizontalVelocity.magnitude;

        bool isGrounded = characterController.isGrounded;
        bool isRunning = playerController.currentCharacterSpeed >= playerController.runSpeed - 0.1f;

        Vector3 targetPosition = initialPosition;

        if (speed > minSpeedToBob && isGrounded)
        {
            // === Bobbing при движении ===
            currentBobAmount = isRunning ? runBobAmount : walkBobAmount;
            currentBobSpeed = isRunning ? runBobSpeed : walkBobSpeed;

            // Умножаем на нормализованную скорость, чтобы bob зависел от реальной скорости
            float speedFactor = Mathf.Clamp01(speed / playerController.runSpeed);
            timer += Time.deltaTime * currentBobSpeed * speedFactor;

            float bobX = Mathf.Cos(timer) * currentBobAmount * horizontalMultiplier;
            float bobY = Mathf.Abs(Mathf.Sin(timer)) * currentBobAmount;

            targetPosition += new Vector3(bobX, bobY, 0f);
        }
        else
        {
            // === Idle Sway ===
            timer += Time.deltaTime * idleSwaySpeed;

            float idleX = Mathf.Sin(timer * 0.7f) * idleSwayAmount;
            float idleY = Mathf.Sin(timer) * idleSwayAmount * 0.6f;

            targetPosition += new Vector3(idleX, idleY, 0f);
        }

        // Плавное применение позиции
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * smooth
        );
    }
}