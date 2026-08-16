using System.Collections.Generic;
using System.Net;
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

    // НОВОЕ: слои, блокирующие обзор (стены, препятствия, земля)
    [Header("Line of Sight")]
    public LayerMask obstacleMask;   // Назначить в инспекторе

    private bool hasLineOfSight = false;

    private bool Dead = false;

    public float maxHealth = 100f;
    public float currentHealth = 100f;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attack
    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    //states
    public float attackRange, sightRange;
    public bool playerInAttackRange, playerInSightRange;

    private enum EnemyState
    {
        Idle,
        Walk,
        Run,
        Fire
    }
    private EnemyState currentState = EnemyState.Idle;

    [SerializeField] public Transform weaponMuzzle;

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
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);


        if (Dead == true)
        {
            return;
        }

        // Проверяем прямую видимость каждый кадр
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
                // Игрок виден – атакуем
                AttackPlayer();
                SetAnimation(EnemyState.Fire);
            }
            else
            {
                // Игрок рядом, но скрыт за препятствием – идём к нему, чтобы найти точку для выстрела
                ChasePlayer();
                SetAnimation(EnemyState.Run);
            }
        }
    }

    private void CheckLineOfSight()
    {
        if (player == null)
        {
            hasLineOfSight = false;
            return;
        }

        // Начало луча – чуть выше центра врага, чтобы не задеть собственный коллайдер
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        // Конец луча – центр игрока (можно тоже приподнять)
        Vector3 target = player.position + Vector3.up * 1.2f;

        Vector3 direction = target - origin;
        float distance = direction.magnitude;

        // Луч бьёт только по obstacleMask, игнорируя игрока и самого врага
        if (Physics.Raycast(origin, direction.normalized, distance, obstacleMask))
        {
            hasLineOfSight = false;
        }
        else
        {
            hasLineOfSight = true;
        }
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
            DrawShotLine();
            TriggerFire();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        Vector3 dirToPlayer = (player.position - weaponObject.transform.position).normalized;
        weaponObject.transform.rotation = Quaternion.LookRotation(dirToPlayer);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        if (playerInSightRange && !playerInAttackRange)
        {
            SetAnimation(EnemyState.Run);
        }
        else if (!playerInSightRange && !playerInAttackRange)
        {
            SetAnimation(EnemyState.Walk);
        }
        else if (playerInAttackRange && playerInSightRange && hasLineOfSight)
        {
            SetAnimation(EnemyState.Fire);
        }
        // Если игрок в зоне, но нет прямой видимости, состояние установится в Update()
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
        if (currentHealth < 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Dead = true;
        anim.SetInteger("Death", Random.Range(0, 4));
        Invoke(nameof(DisableAnimatorAndObject), 0.7f);
    }

    private void DisableAnimatorAndObject()
    {
        anim.SetBool("Dead", true);
        this.enabled = false;
        //anim.enabled = false;
        //gameObject.SetActive(false);
    }





    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Отображение прямой видимости
        if (player != null)
        {
            Gizmos.color = hasLineOfSight ? Color.green : Color.magenta;
            Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, player.position + Vector3.up * 1.2f);
        }
    }

    private void InitializeTracePool()
    {
        for (int i = 0; i < tracePoolSize; i++)
        {
            GameObject obj = Instantiate(tracePrefab);
            obj.SetActive(false);
            TraceEffect effect = obj.GetComponent<TraceEffect>();
            if (effect != null)
            {
                TraceEffect captured = effect;
                captured.OnComplete += (captured) => tracePool.Enqueue(captured);
                tracePool.Enqueue(captured);
            }
        }
    }

    private void DrawShotLine()
    {
        if (tracePool.Count > 0)
        {
            TraceEffect effect = tracePool.Dequeue();
            effect.Play(weaponMuzzle.position, player.transform.position);
        }
    }

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