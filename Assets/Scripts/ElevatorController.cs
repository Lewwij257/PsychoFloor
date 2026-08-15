using UnityEngine;

public class ElevatorController : MonoBehaviour
{

    [SerializeField] Animator door1Animator;
    [SerializeField] Animator door2Animator;

    [SerializeField] TriggerZone inFrontOfDoors;
    [SerializeField] TriggerZone inElevator;

    [SerializeField] public LayerMask playerLayer;




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


    private void OnPlayerEnterFront(Collider other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0) return;
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
