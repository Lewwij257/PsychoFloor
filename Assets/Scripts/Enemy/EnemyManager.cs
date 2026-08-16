using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [Header("Movement")]
    public float currentCharacterSpeed;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    [Header("Other")]
    [SerializeField] public GameManager gameManager;

    [Header("Bullet visual")]
    [SerializeField] private GameObject tracePrefab;
    [SerializeField] private int tracePoolSize = 10;
    private Queue<TraceEffect> tracePool = new Queue<TraceEffect>();

    [SerializeField] public GameObject weaponObject;
    public GameObject testProjectile;

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    [Header("Line of Sight")]
    public LayerMask obstacleMask;

    private bool hasLineOfSight = false;
    private bool Dead = false;

    public float maxHealth = 100f;
    public float currentHealth = 100f;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attack
    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    // states
    public float attackRange, sightRange;
    public bool playerInAttackRange, playerInSightRange;

    // ===== НОВЫЕ ПОЛЯ ДЛЯ СТРЕЛЬБЫ =====
    [Header("Shooting")]
    public float damage = 10f;              // урон за выстрел
    public float shootRange = 100f;         // дальность рейкаста
    public float spreadAngle = 5f;          // разброс в градусах
    public LayerMask shootableLayers;       // маска для рейкаста (игрок, стены, препятствия)

    [SerializeField] public Transform weaponMuzzle;

    private enum EnemyState { Idle, Walk, Run, Fire }
    private EnemyState currentState = EnemyState.Idle;

    private Animator anim;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        InitializeTracePool();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        player = GameManager.Instance.Player.transform;
        if (gameManager == null) gameManager = GameManager.Instance;
        GameManager.Instance?.RegisterEnemy(this); // <-- добавить
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (Dead) return;

        CheckLineOfSight();

        agent.updateRotation = true;

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
            SetAnimation(EnemyState.Walk);
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
            SetAnimation(EnemyState.Run);
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            if (hasLineOfSight)
            {
                AttackPlayer();
                SetAnimation(EnemyState.Fire);
            }
            else
            {
                ChasePlayer();
                SetAnimation(EnemyState.Run);
            }
        }
    }

    private void CheckLineOfSight()
    {
        if (player == null) { hasLineOfSight = false; return; }

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 target = player.position + Vector3.up * 1.2f;
        Vector3 direction = target - origin;
        float distance = direction.magnitude;

        if (Physics.Raycast(origin, direction.normalized, distance, obstacleMask))
            hasLineOfSight = false;
        else
            hasLineOfSight = true;
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1)
        {
            walkPointSet = false;
            SetAnimation(EnemyState.Idle);
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    // ========== ИЗМЕНЁННЫЙ AttackPlayer ==========
    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        agent.updateRotation = false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        if (!alreadyAttacked)
        {
            PerformShot();          // теперь стреляем через рейкаст
            TriggerFire();          // анимация выстрела
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        Vector3 dirToPlayer = (player.position - weaponObject.transform.position).normalized;
        weaponObject.transform.rotation = Quaternion.LookRotation(dirToPlayer);
    }

    // ========== НОВЫЙ МЕТОД СТРЕЛЬБЫ ==========
    private void PerformShot()
    {
        if (player == null) return;

        // Направление от дула к игроку
        Vector3 baseDirection = (player.position - weaponMuzzle.position).normalized;

        // Применяем разброс
        Vector3 shootDirection = ApplySpread(baseDirection, spreadAngle);

        Ray ray = new Ray(weaponMuzzle.position, shootDirection);
        RaycastHit hit;
        Vector3 hitPoint;
        bool hitSomething = false;

        if (Physics.Raycast(ray, out hit, shootRange, shootableLayers))
        {
            hitPoint = hit.point;
            hitSomething = true;

            // ---- Определение поверхности (если используется) ----
            SurfaceType surface = SurfaceType.Default;
            var surfaceIdentifier = hit.collider.GetComponent<SurfaceIdentifier>();
            if (surfaceIdentifier != null)
                surface = surfaceIdentifier.SurfaceType;

            // ---- Спавн импакта ----
            Quaternion impactRot = Quaternion.LookRotation(-ray.direction, hit.normal);
            // ImpactManager – предполагается синглтон, если его нет – закомментируйте или замените на Instantiate
            if (ImpactManager.Instance != null)
                ImpactManager.Instance.SpawnImpact(hit.point, impactRot, surface);

            // ---- Проверка попадания в игрока ----
            if (hit.collider.CompareTag("Player"))
            {
                // Получаем компонент здоровья игрока (подставьте свой класс)
                PlayerController playerController = hit.collider.GetComponentInParent<PlayerController>();
                if (playerController != null)
                    playerController.TakeDamage(40);
            }
        }
        else
        {
            // Если никуда не попали – конечная точка на максимальной дистанции
            hitPoint = weaponMuzzle.position + shootDirection * shootRange;
        }

        // Рисуем трассер (линию от дула до точки попадания)
        DrawShotLine(weaponMuzzle.position, hitPoint);
    }

    // ========== РАЗБРОС ==========
    private Vector3 ApplySpread(Vector3 direction, float spreadAngleDegrees)
    {
        if (spreadAngleDegrees <= 0) return direction.normalized;

        float angleRad = spreadAngleDegrees * Mathf.Deg2Rad;
        float radius = Mathf.Sqrt(Random.Range(0f, 1f)) * angleRad;
        float theta = Random.Range(0f, 2f * Mathf.PI);

        // Строим ортогональный базис
        Vector3 up = Vector3.up;
        if (Mathf.Abs(Vector3.Dot(direction, up)) > 0.99f)
            up = Vector3.right;

        Vector3 right = Vector3.Cross(direction, up).normalized;
        Vector3 localUp = Vector3.Cross(right, direction).normalized;

        Vector3 offset = (right * Mathf.Cos(theta) + localUp * Mathf.Sin(theta)) * radius;
        return (direction + offset).normalized;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        if (playerInSightRange && !playerInAttackRange)
            SetAnimation(EnemyState.Run);
        else if (!playerInSightRange && !playerInAttackRange)
            SetAnimation(EnemyState.Walk);
        else if (playerInAttackRange && playerInSightRange && hasLineOfSight)
            SetAnimation(EnemyState.Fire);
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
            walkPointSet = true;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) Die();
    }

    public void Die()
    {
        Dead = true;
        anim.SetInteger("Death", Random.Range(0, 4));
        Invoke(nameof(DisableAnimatorAndObject), 0.7f);
        agent.enabled = false;
        GameManager.Instance?.UnregisterEnemy(this); // <-- добавить
        GameManager.Instance.enemiesKilledOnCurrentFloor += 1; // <-- добавить
    }

    private void DisableAnimatorAndObject()
    {
        anim.SetBool("Dead", true);
        this.enabled = false;
    }

    public void DestroyEnemy() => Destroy(gameObject);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        if (player != null)
        {
            Gizmos.color = hasLineOfSight ? Color.green : Color.magenta;
            Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, player.position + Vector3.up * 1.2f);
        }
    }

    // ========== ПУЛ ТРАССЕРОВ (переработан) ==========
    private void InitializeTracePool()
    {
        for (int i = 0; i < tracePoolSize; i++)
        {
            GameObject obj = Instantiate(tracePrefab);
            obj.SetActive(false);
            TraceEffect effect = obj.GetComponent<TraceEffect>();
            if (effect != null)
            {
                effect.OnComplete += (e) => tracePool.Enqueue(e);
                tracePool.Enqueue(effect);
            }
        }
    }

    // Теперь принимает начальную и конечную точку
    private void DrawShotLine(Vector3 start, Vector3 end)
    {
        if (tracePool.Count > 0)
        {
            TraceEffect effect = tracePool.Dequeue();
            effect.Play(start, end);
        }
    }

    // ========== ПОЗИЦИЯ ОРУЖИЯ И АНИМАЦИИ ==========
    public void SetWeaponPosition(int state)
    {
        Transform weapon = weaponObject.transform;
        switch (state)
        {
            case 0: // Idle
                weapon.localPosition = new Vector3(-0.0349f, 0.1251f, 0.0383f);
                weapon.localRotation = Quaternion.Euler(-98.664f, -15.914f, 105.057f);
                break;
            case 1: // Walk
                weapon.localPosition = new Vector3(-0.0413f, 0.1164f, 0.0334f);
                weapon.localRotation = Quaternion.Euler(-92.278f, -101.168f, 190.139f);
                break;
            case 2: // Fire
                weapon.localPosition = new Vector3(-0.0297f, 0.1465f, 0.0401f);
                weapon.localRotation = Quaternion.Euler(-96.709f, -53.65698f, 146.968f);
                break;
            case 3: // Run
                weapon.localPosition = new Vector3(-0.0297f, 0.1465f, 0.0401f);
                weapon.localRotation = Quaternion.Euler(-96.709f, -53.65698f, 146.968f);
                break;
        }
    }

    private void SetAnimation(EnemyState newState)
    {
        if (currentState == newState) return;

        anim.ResetTrigger("Idle");
        anim.ResetTrigger("Walk");
        anim.ResetTrigger("Run");
        anim.ResetTrigger("Fire");

        switch (newState)
        {
            case EnemyState.Idle:
                SetWeaponPosition(0);
                anim.SetTrigger("Idle");
                break;
            case EnemyState.Walk:
                anim.SetTrigger("Walk");
                SetWeaponPosition(1);
                break;
            case EnemyState.Run:
                anim.SetTrigger("Run");
                SetWeaponPosition(3);
                break;
            case EnemyState.Fire:
                anim.SetTrigger("Fire");
                SetWeaponPosition(2);
                break;
        }
        currentState = newState;
    }

    public void TriggerFire() => SetAnimation(EnemyState.Fire);
    public void TriggerWalk() => SetAnimation(EnemyState.Walk);
    public void TriggerRun() => SetAnimation(EnemyState.Run);
    public void TriggerIdle() => SetAnimation(EnemyState.Idle);
}