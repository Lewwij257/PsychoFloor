using TMPro;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{

    [SerializeField] Animator door1Animator;
    [SerializeField] Animator door2Animator;

    [SerializeField] TriggerZone inFrontOfDoors;
    [SerializeField] TriggerZone inElevator;

    [SerializeField] public LayerMask playerLayer;


    [SerializeField] public TextMeshProUGUI killsText;
    [SerializeField] public TextMeshProUGUI HeadshotsText;
    [SerializeField] public TextMeshProUGUI HitsText;
    [SerializeField] public TextMeshProUGUI TimeText;
    [SerializeField] public TextMeshProUGUI DamageDealed;
    [SerializeField] public TextMeshProUGUI DamageTaken;
    [SerializeField] public TextMeshProUGUI Score;



    private void Start()
    {


        if (inFrontOfDoors != null)
        {
            inFrontOfDoors.onTriggerEnter.AddListener(OnPlayerEnterFront);
            inFrontOfDoors.onTriggerExit.AddListener(OnPlayerExitFront);
        }

        if (inElevator != null)
        {
            inElevator.onTriggerEnter.AddListener(OnPlayerEnterElevator);
            inElevator.onTriggerExit.AddListener(OnPlayerExitElevator);
        }
    }

    private void ShowStatsOnUI()
    {
        // Получаем менеджер
        GameManager gm = GameManager.Instance;

        // Останавливаем таймер (если ещё не остановлен)
        gm.StopTimer();

        // Заполняем тексты
        killsText.text = "Убийств: " + gm.enemiesKilledOnCurrentFloor.ToString();
        HeadshotsText.text = "Хэдшотов: " + gm.headshotsOnCurrentFloor.ToString();
        HitsText.text = "Попаданий: " + gm.hitsOnCurrentFloor.ToString();
        DamageDealed.text = "Нанесено урона: " + gm.damageDealedOnCurrentFloor.ToString();
        DamageTaken.text = "Получено урона: " + gm.damageTakenOnCurrentFloor.ToString();
        TimeText.text = "Время: " + gm.elapsedTime.ToString("F1") + " сек";

        // Расчёт оценки
        int score = gm.CalculateScore();
        string rating = gm.GetRating(score);
        string message = gm.GetRatingMessage(score);

        Score.text = $"Оценка: {rating} ({score})";
    }

    private void OnPlayerEnterFront(Collider other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0) return;

        if (GameManager.Instance.Enemies.Count > 0) return;

        OpenDoors();

    }

    private void OnPlayerExitFront(Collider other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0) return;

        CloseDoors();
    }

    private void OnPlayerEnterElevator(Collider other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0) return;
        ShowStatsOnUI();
        CloseDoors();
    }

    private void OnPlayerExitElevator(Collider other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0) return;


    }


    public void OpenDoors()
    {
        door1Animator.SetTrigger("OpenDoors");
        door2Animator.SetTrigger("OpenDoors");
    }

    public void CloseDoors()
    {
        door1Animator.SetTrigger("CloseDoors");
        door2Animator.SetTrigger("CloseDoors");
    }
}
