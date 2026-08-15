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


    public GameObject testProjectile;

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask playerLayer;
    public LayerMask groundLayer;

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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        InitializeTracePool();
    }

    private void Start()
    {
        player = GameManager.Instance.Player.transform;
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();


    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1)  walkPointSet = false;
    }


    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {


            ///attack code

            //Rigidbody rb = Instantiate(testProjectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            //rb.AddForce(transform.forward * 8f, ForceMode.Impulse);

            DrawShotLIne();


            ///

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;

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

        if (currentHealth < 0) Invoke(nameof(DestroyEnemy), 2f);
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

    private void DrawShotLIne()
    {
        if (tracePool.Count > 0)
        {
            TraceEffect effect = tracePool.Dequeue();
            effect.Play(transform.position, player.transform.position);
        }
    }


}
