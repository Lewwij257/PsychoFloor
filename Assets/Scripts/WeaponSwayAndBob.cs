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
    public float minSpeedToBob = 0.15f;

    [Header("Настройки отдачи (Recoil)")]
    public float recoilAmount = 0.03f;
    public float recoilRecoverySpeed = 12f;
    public float recoilDuration = 0.15f;
    public float recoilRotationAmount = 3f;

    [Header("Эффект выстрела")]
    public GameObject muzzleFlashPrefab;
    public Transform muzzlePoint;
    public float flashLifeTime = 0.05f;          // ОЧЕНЬ мало (50 миллисекунд)

    [Header("Ссылки")]
    public PlayerController playerController;
    public CharacterController characterController;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float timer;
    private float currentBobAmount;
    private float currentBobSpeed;

    private Vector3 recoilOffset;
    private Vector3 recoilRotationOffset;
    private float recoilTimer;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        if (playerController == null)
            playerController = GetComponentInParent<PlayerController>();

        if (characterController == null)
            characterController = GetComponentInParent<CharacterController>();

        if (muzzleFlashPrefab == null)
            Debug.LogWarning("Muzzle Flash Prefab не назначен в инспекторе!");

        if (muzzlePoint == null)
            Debug.LogWarning("Muzzle Point не назначен в инспекторе!");
    }

    void Update()
    {
        if (playerController == null || characterController == null)
            return;

        if (recoilTimer > 0)
        {
            recoilTimer -= Time.deltaTime;
        }

        Vector3 horizontalVelocity = new Vector3(
            characterController.velocity.x,
            0f,
            characterController.velocity.z
        );
        float speed = horizontalVelocity.magnitude;

        bool isGrounded = characterController.isGrounded;
        bool isRunning = playerController.currentCharacterSpeed >= playerController.runSpeed - 0.1f;

        Vector3 targetPosition = initialPosition;
        Quaternion targetRotation = initialRotation;

        if (speed > minSpeedToBob && isGrounded)
        {
            currentBobAmount = isRunning ? runBobAmount : walkBobAmount;
            currentBobSpeed = isRunning ? runBobSpeed : walkBobSpeed;

            float speedFactor = Mathf.Clamp01(speed / playerController.runSpeed);
            timer += Time.deltaTime * currentBobSpeed * speedFactor;

            float bobX = Mathf.Cos(timer) * currentBobAmount * horizontalMultiplier;
            float bobY = Mathf.Abs(Mathf.Sin(timer)) * currentBobAmount;

            targetPosition += new Vector3(bobX, bobY, 0f);
        }
        else
        {
            timer += Time.deltaTime * idleSwaySpeed;

            float idleX = Mathf.Sin(timer * 0.7f) * idleSwayAmount;
            float idleY = Mathf.Sin(timer) * idleSwayAmount * 0.6f;

            targetPosition += new Vector3(idleX, idleY, 0f);
        }

        if (recoilTimer > 0)
        {
            float recoilProgress = 1f - (recoilTimer / recoilDuration);
            float recoilCurve = Mathf.Sin(recoilProgress * Mathf.PI);

            targetPosition += recoilOffset * recoilCurve;
            targetRotation *= Quaternion.Euler(recoilRotationOffset * recoilCurve);
        }
        else
        {
            recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
            recoilRotationOffset = Vector3.Lerp(recoilRotationOffset, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
        }

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * smooth
        );

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * smooth
        );
    }

    public void Shoot()
    {
        // Отдача
        recoilOffset = new Vector3(
            Random.Range(-0.005f, 0.005f),
            recoilAmount * 1.2f,
            -recoilAmount * 0.3f
        );

        recoilOffset.x += Random.Range(-0.005f, 0.005f);

        recoilRotationOffset = new Vector3(
            -recoilRotationAmount * (1f + Random.Range(-0.1f, 0.1f)),
            Random.Range(-0.3f, 0.3f),
            Random.Range(-0.2f, 0.2f)
        );

        recoilTimer = recoilDuration;

        // Эффект выстрела
        SpawnMuzzleFlash();
    }

    private void SpawnMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || muzzlePoint == null)
            return;

        // Создаём эффект как ДОЧЕРНИЙ объект
        GameObject flashInstance = Instantiate(
            muzzleFlashPrefab,
            muzzlePoint.position,
            muzzlePoint.rotation,
            transform  // Родитель - это оружие (текущий объект)
        );

        // Получаем ParticleSystem
        ParticleSystem particleSystem = flashInstance.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            // Запускаем частицы
            particleSystem.Play();

            // Уничтожаем через ОЧЕНЬ КОРОТКОЕ время (если частицы не остановятся сами)
            float lifetime = Mathf.Min(flashLifeTime, particleSystem.main.duration);
            Destroy(flashInstance, lifetime);
        }
        else
        {
            // Если ParticleSystem нет, уничтожаем через указанное время
            Destroy(flashInstance, flashLifeTime);
        }
    }
}