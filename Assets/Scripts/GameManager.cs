using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LayerMask PlayerLayer;

    public int enemiesKilledOnCurrentFloor;
    public int damageDealedOnCurrentFloor;
    public int headshotsOnCurrentFloor;
    public int damageTakenOnCurrentFloor;
    public int hitsOnCurrentFloor;
    public int shotsOnCurrentFloor;
    public float startTime;
    public float stopTime;

    public GameObject DeathPanel;

    public static GameManager Instance { get; private set; }

    [SerializeField] public GameObject Player;

    // Список всех живых врагов
    public List<EnemyManager> Enemies = new List<EnemyManager>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Явный поиск не требуется – враги сами регистрируются в Start
        // EnemyManager[] enemiesInLevel = FindObjectsByType<EnemyManager>();
    }

    public void GameOver()
    {
        DeathPanel.SetActive(true);
    }

    /// LEVEL/ GAME? STATISTICKS
    /// 


    public void SaveStatistics()
    {

    }

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
        hitsOnCurrentFloor += 1;
    }

    public void RegisterShot()
    {
        shotsOnCurrentFloor += 1;
    }

    public void StartRegisterTime()
    {
        startTime = Time.time;
    }

    public void StopRegisterTime()
    {
        stopTime = Time.time;
    }

    // Добавление врага в список
    public void RegisterEnemy(EnemyManager enemy)
    {
        if (!Enemies.Contains(enemy))
            Enemies.Add(enemy);
    }

    // Удаление врага из списка (при смерти)
    public void UnregisterEnemy(EnemyManager enemy)
    {
        if (Enemies.Contains(enemy))
            Enemies.Remove(enemy);
    }
}