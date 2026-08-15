using UnityEngine;

public class GameManager : MonoBehaviour
{

    public LayerMask PlayerLayer;


    public int enemiesKilledOnCurrentFloor;
    public int damageDealedOnCurrentFloor;
    public int headshotsOnCurrentFloor;
    public int damageTakenOnCurrentFloor;
    public int hitsOnCurrentFloor;
    public int missesOnCurrentFloor;
    public float startTime;
    public float stopTime;


    public static GameManager Instance { get; private set; }

    [SerializeField] public GameObject Player;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }





    /// LEVEL/ GAME? STATISTICKS
    /// 


    public void RegisterEnemyKilled()
    {
        enemiesKilledOnCurrentFloor++;
    }

    public void RegisterDealDamage(int damage)
    {
        damageDealedOnCurrentFloor += damage;
    }

    public void RegisterHeadshot()
    {
        headshotsOnCurrentFloor += 1;
    }

    public void RegisterDamageTaken(int damage)
    {
        damageTakenOnCurrentFloor += damage;
    }

    public void RegisterHit()
    {
        hitsOnCurrentFloor+= 1; 
    }

    public void RegisterMiss()
    {
        missesOnCurrentFloor+= 1;
    }

    public void StartRegisterTime()
    {
        startTime = Time.time;
    }

    public void StopRegisterTime()
    {
        stopTime = Time.time;
    }

}