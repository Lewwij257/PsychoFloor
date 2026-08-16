using System.Collections.Generic;
using TMPro;
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
    public TextMeshProUGUI MagPanel;

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

    private void Start()
    {

    }


    private void Update()
    {
    }


    public float elapsedTime; // время прохождения этажа

    public void StartTimer()
    {
        startTime = Time.time;
    }

    public void StopTimer()
    {
        stopTime = Time.time;
        elapsedTime = stopTime - startTime;
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


    public int CalculateScore()
    {
        // 1. Точность
        float accuracy = (shotsOnCurrentFloor > 0)
            ? Mathf.Clamp01((float)hitsOnCurrentFloor / shotsOnCurrentFloor)
            : 0f;

        // 2. Хэдшоты
        float headshotRatio = (enemiesKilledOnCurrentFloor > 0)
            ? Mathf.Clamp01((float)headshotsOnCurrentFloor / enemiesKilledOnCurrentFloor)
            : 0f;

        // 3. Соотношение урона
        float damageRatio = (damageTakenOnCurrentFloor + 1 > 0)
            ? Mathf.Min((float)damageDealedOnCurrentFloor / (damageTakenOnCurrentFloor + 1), 5f) / 5f
            : 0f;

        // 4. Бонус за время (максимум 120 секунд)
        float timeBonus = Mathf.Clamp01(1f - elapsedTime / 120f);

        // 5. Базовый счёт
        float score = accuracy * 30f + headshotRatio * 20f + damageRatio * 30f + timeBonus * 20f;

        // 6. Штраф за оставшихся врагов
        if (Enemies.Count > 0)
            score -= 20f;

        // 7. Приводим к целому и зажимаем
        return Mathf.Clamp(Mathf.RoundToInt(score), 0, 100);
    }

    public string GetRating(int score)
    {
        if (score >= 90) return "S";
        if (score >= 75) return "A";
        if (score >= 60) return "B";
        if (score >= 40) return "C";
        if (score >= 20) return "D";
        return "F";
    }

    public string GetRatingMessage(int score)
    {
        if (score >= 90) return "Идеальный забег! Вы – офисный ниндзя!";
        if (score >= 75) return "Отлично! Вы эффективны и смертоносны.";
        if (score >= 60) return "Хорошо, но есть куда расти.";
        if (score >= 40) return "Средненько. Попробуйте быстрее и точнее.";
        if (score >= 20) return "Плохо. Вас выгонят с такой эффективностью.";
        return "Позор! Вы даже не справились с офисом!";
    }
}