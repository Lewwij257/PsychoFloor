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
        killsText.text += GameManager.Instance.enemiesKilledOnCurrentFloor.ToString();
        HeadshotsText.text += GameManager.Instance.headshotsOnCurrentFloor.ToString();
        HitsText.text += GameManager.Instance.hitsOnCurrentFloor.ToString();
        TimeText.text += 0.ToString();
        DamageDealed.text += GameManager.Instance.damageDealedOnCurrentFloor.ToString();
        DamageTaken.text += GameManager.Instance.damageTakenOnCurrentFloor.ToString();
        Score.text += "!";

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
